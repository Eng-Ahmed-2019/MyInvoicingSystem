using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.EntityFrameworkCore;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyReadDto?> GetMyCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("CompanyId is invalid.");

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null) throw new KeyNotFoundException($"Company with ID {companyId} was not found.");

            return new CompanyReadDto
            {
                Id = company.Id,
                Name = company.Name,
                NameAr = company.NameAr,
                Description = company.Description,
                DescriptionAr = company.DescriptionAr,
                CreatedAt = company.CreatedAt
            };
        }

        public async Task<CompantDTO?> GetCompanyByIdAndNameAsync(Guid id, string name)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Company ID is invalid.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Company name is invalid.");

            var company = await _context.Companies
                .AsNoTracking()
                .Where(c => c.Id == id && (c.Name == name || c.NameAr == name))
                .Select(c => new CompantDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    NameAr = c.NameAr,
                    Description = c.Description ?? string.Empty,
                    DescriptionAr = c.DescriptionAr ?? string.Empty
                })
                .FirstOrDefaultAsync();

            if (company == null)
                throw new KeyNotFoundException($"Company with ID {id} and Name '{name}' was not found.");

            return company;
        }

        public async Task<CompanyResultDto?> CreateCompanyAsync(CompanyCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Company data must not be null.");

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.NameAr))
                throw new ArgumentException("Company name (English/Arabic) must not be empty.");

            var exists = await _context.Companies
                .AnyAsync(c => c.Name == dto.Name || c.NameAr == dto.NameAr);

            if (exists)
                throw new InvalidOperationException($"Company with name '{dto.Name}' or '{dto.NameAr}' already exists.");

            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                NameAr = dto.NameAr.Trim(),
                Description = dto.Description?.Trim(),
                DescriptionAr = dto.DescriptionAr?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return new CompanyResultDto
            {
                Id = company.Id,
                Name = company.Name,
                NameAr = company.NameAr,
                Description = company.Description ?? string.Empty,
                DescriptionAr = company.DescriptionAr ?? string.Empty
            };
        }

        public async Task<CompanyResultDto?> UpdateCompanyAsync(Guid id, CompanyUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.NameAr))
                throw new ArgumentException("اسم الشركة واسم الشركة بالعربية مطلوبان.");

            if (dto.Name.Length > 100 || dto.NameAr.Length > 100)
                throw new ArgumentException("اسم الشركة لا يجب أن يتجاوز 100 حرف.");

            var company = await _context.Companies.FindAsync(id);
            if (company == null) return null;

            var conflict = await _context.Companies
                .AnyAsync(c => c.Id != id && (c.Name == dto.Name || c.NameAr == dto.NameAr));
            if (conflict)
                throw new InvalidOperationException("يوجد شركة أخرى بنفس الاسم أو الاسم العربي.");

            company.Name = dto.Name;
            company.NameAr = dto.NameAr;
            company.Description = dto.Description ?? string.Empty;
            company.DescriptionAr = dto.DescriptionAr ?? string.Empty;
            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CompanyResultDto
            {
                Id = company.Id,
                Name = company.Name,
                NameAr = company.NameAr,
                Description = company.Description,
                DescriptionAr = company.DescriptionAr
            };
        }

        public async Task<bool> DeleteCompanyAsync(Guid id)
        {
            try
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null) return false;

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting company: {ex.Message}");
                return false;
            }
        }
    }
}