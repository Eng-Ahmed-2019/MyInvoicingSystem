using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.EntityFrameworkCore;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserReadDto>> GetUsersAsync(Guid companyId)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.CompanyId == companyId)
                .Include(u => u.Role)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    FullNameAr = u.FullNameAr,
                    RoleId = u.RoleId,
                    RoleName = u.Role != null ? u.Role.NameEn : string.Empty,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<UserReadDto?> GetUserByIdAsync(Guid companyId, Guid id)
        {
            var u = await _context.Users
                .AsNoTracking()
                .Include(x => x.Role)
                .Where(x => x.Id == id && x.CompanyId == companyId)
                .Select(x => new UserReadDto
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    FullName = x.FullName,
                    FullNameAr = x.FullNameAr,
                    RoleId = x.RoleId,
                    RoleName = x.Role != null ? x.Role.NameEn : string.Empty,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            return u;
        }

        public async Task<UserReadDto?> CreateUserAsync(Guid companyId, UserCreateDto dto)
        {
            var exists = await _context.Users.AnyAsync(u =>
                u.CompanyId == companyId &&
                (u.Email == dto.Email || u.Username == dto.Username));

            if (exists) return null;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == dto.RoleId && r.CompanyId == companyId);

            var hashed = PasswordHasher.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hashed,
                FullName = dto.FullName,
                FullNameAr = dto.FullNameAr,
                CompanyId = companyId,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                FullNameAr = user.FullNameAr,
                CompanyId = user.CompanyId,
                RoleId = user.RoleId,
                RoleName = role?.NameEn ?? string.Empty,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<(bool Success, bool Conflict)> UpdateUserAsync(Guid companyId, Guid id, UserUpdateDto dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.CompanyId == companyId);
            if (existingUser == null) return (false, false);

            var conflict = await _context.Users.AnyAsync(u =>
                u.CompanyId == companyId &&
                u.Id != id &&
                (u.Email == dto.Email || u.Username == dto.Username));

            if (conflict) return (false, true);

            existingUser.FullName = dto.FullName;
            existingUser.FullNameAr = dto.FullNameAr;
            existingUser.Email = dto.Email;
            existingUser.Username = dto.Username;
            existingUser.RoleId = dto.RoleId;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                existingUser.PasswordHash = PasswordHasher.HashPassword(dto.Password);
            }

            existingUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, false);
        }

        public async Task<bool> DeleteUserAsync(Guid companyId, Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.CompanyId == companyId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}