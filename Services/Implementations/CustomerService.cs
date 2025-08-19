using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.EntityFrameworkCore;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerReadDto>> GetCustomersAsync(Guid companyId)
        {
            var companyExists = await _context.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
            {
                return Enumerable.Empty<CustomerReadDto>();
            }

            return await _context.Customers
                .AsNoTracking()
                .Where(c => c.CompanyId == companyId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CustomerReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    NameAr = c.NameAr,
                    Email = c.Email,
                    Phone = c.Phone,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CustomerReadDto?> CreateCustomerAsync(Guid companyId, CustomerCreateDto dto)
        {
            var companyExists = await _context.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
                throw new InvalidOperationException("الشركة غير موجودة.");

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.NameAr))
                throw new ArgumentException("اسم العميل واسم العميل بالعربية مطلوبان.");

            var exists = await _context.Customers.AnyAsync(c =>
                c.CompanyId == companyId &&
                ((dto.Email != null && c.Email == dto.Email) ||
                 (dto.Phone != null && c.Phone == dto.Phone)));

            if (exists)
                throw new InvalidOperationException("يوجد عميل بنفس البريد الإلكتروني أو رقم الهاتف داخل هذه الشركة.");

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                NameAr = dto.NameAr,
                Email = dto.Email,
                Phone = dto.Phone,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return new CustomerReadDto
            {
                Id = customer.Id,
                Name = customer.Name,
                NameAr = customer.NameAr,
                Email = customer.Email,
                Phone = customer.Phone,
                CreatedAt = customer.CreatedAt
            };
        }

        public async Task<CustomerUpdateStatus> EditCustomerAsync(Guid companyId, Guid id, CustomerUpdateDto dto)
        {
            var companyExists = await _context.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
                return CustomerUpdateStatus.NotFound;

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (customer == null)
                return CustomerUpdateStatus.NotFound;

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.NameAr))
                return CustomerUpdateStatus.InvalidData;

            if (dto.Name.Length > 100 || dto.NameAr.Length > 100)
                return CustomerUpdateStatus.InvalidData;

            if (dto.Phone != null && dto.Phone.Length > 50)
                return CustomerUpdateStatus.InvalidData;

            if (dto.Email != null && dto.Email.Length > 100)
                return CustomerUpdateStatus.InvalidData;

            var conflict = await _context.Customers.AnyAsync(c =>
                    c.CompanyId == companyId &&
                    c.Id != id &&
                    ((dto.Email != null && c.Email == dto.Email) ||
                     (dto.Phone != null && c.Phone == dto.Phone)));

            if (conflict)
                return CustomerUpdateStatus.Conflict;

            customer.Name = dto.Name;
            customer.NameAr = dto.NameAr;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return CustomerUpdateStatus.Success;
        }

        public async Task<bool> DeleteCustomerAsync(Guid companyId, Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}