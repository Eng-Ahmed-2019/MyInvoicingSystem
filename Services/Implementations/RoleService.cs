using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.EntityFrameworkCore;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync(Guid companyId)
        {
            return await _context.Roles
                .Where(r => r.CompanyId == companyId)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.NameEn,
                    NameAr = r.NameAr,
                    Description = r.DescriptionEn ?? string.Empty,
                    DescriptionAr = r.DescriptionAr ?? string.Empty,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<RoleDto?> GetRoleByIdAsync(Guid companyId, Guid id)
        {
            return await _context.Roles
                .Where(r => r.Id == id && r.CompanyId == companyId)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.NameEn,
                    NameAr = r.NameAr,
                    Description = r.DescriptionEn ?? string.Empty,
                    DescriptionAr = r.DescriptionAr ?? string.Empty,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<RoleDto?> CreateRoleAsync(Guid companyId, RoleDto roleDto)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                NameEn = roleDto.Name,
                NameAr = roleDto.NameAr,
                DescriptionEn = roleDto.Description,
                DescriptionAr = roleDto.DescriptionAr,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.NameEn,
                NameAr = role.NameAr,
                Description = role.DescriptionEn ?? string.Empty,
                DescriptionAr = role.DescriptionAr ?? string.Empty,
                CreatedAt = role.CreatedAt
            };
        }

        public async Task<bool> UpdateRoleAsync(Guid companyId, Guid id, RoleDto roleDto)
        {
            var exRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == companyId);

            if (exRole == null) return false;

            exRole.NameEn = roleDto.Name;
            exRole.NameAr = roleDto.NameAr;
            exRole.DescriptionEn = roleDto.Description;
            exRole.DescriptionAr = roleDto.DescriptionAr;
            exRole.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoleAsync(Guid companyId, Guid id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == companyId);

            if (role == null) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}