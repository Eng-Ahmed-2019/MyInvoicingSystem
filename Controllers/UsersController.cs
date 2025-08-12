using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using InvoicingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;

namespace InvoicingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public UsersController(ApplicationDbContext context, IStringLocalizer<Messages> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        private (bool isValid, Guid companyId) GetCompanyId()
        {
            Guid companyId;

            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && Guid.TryParse(companyIdClaim, out companyId))
            {
                return (true, companyId);
            }

            if (Request.Headers.TryGetValue("X-Company-Id", out var companyIdHeader) &&
                Guid.TryParse(companyIdHeader, out companyId))
            {
                return (true, companyId);
            }

            var companyIdString = HttpContext.Items["CompanyId"]?.ToString();
            if (!string.IsNullOrEmpty(companyIdString) && Guid.TryParse(companyIdString, out companyId))
            {
                return (true, companyId);
            }

            return (false, Guid.Empty);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest("Company ID is missing or invalid.");

            var users = await _context.Users
                .Where(u => u.CompanyId == companyId)
                .Include(u => u.Role)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    FullNameAr = u.FullNameAr,
                    RoleName = u.Role!.NameEn,
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadDto>> GetUserById(Guid id)
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest("Company ID is missing or invalid.");

            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Id == id && u.CompanyId == companyId)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    FullNameAr = u.FullNameAr,
                    RoleName = u.Role!.NameEn,
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found or does not belong to your company.");

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadDto>> CreateUser(UserCreateDto dto)
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest("Company ID is missing or invalid.");

            if (await _context.Users.AnyAsync(u => u.CompanyId == companyId &&
                                                  (u.Email == dto.Email || u.Username == dto.Username)))
            {
                return Conflict("A user with the same email or username already exists in this company.");
            }

            var hashedPassword = PasswordHasher.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                FullName = dto.FullName,
                FullNameAr = dto.FullNameAr,
                CompanyId = companyId,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FindAsync(dto.RoleId);

            var readDto = new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                FullNameAr = user.FullNameAr,
                RoleName = role?.NameEn ?? "",
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, readDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto dto)
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest("Company ID is missing or invalid.");

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.CompanyId == companyId);

            if (existingUser == null)
                return NotFound("User not found or does not belong to your company.");

            if ((existingUser.Email != dto.Email || existingUser.Username != dto.Username) &&
                await _context.Users.AnyAsync(u => u.CompanyId == companyId &&
                                                  (u.Email == dto.Email || u.Username == dto.Username) &&
                                                  u.Id != id))
            {
                return Conflict("Another user with the same email or username already exists.");
            }

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

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest("Company ID is missing or invalid.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.CompanyId == companyId);

            if (user == null)
                return NotFound("User not found or does not belong to your company.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}