using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using InvoicingSystem.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace InvoicingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IStringLocalizer<Messages> _localizer;

        public AuthController(ApplicationDbContext context, JwtService jwtService, IStringLocalizer<Messages> localizer)
        {
            _context = context;
            _jwtService = jwtService;
            _localizer = localizer;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.UsernameOrEmail || u.Username == dto.UsernameOrEmail);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (!PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = _jwtService.GenerateToken(
                user.Id,
                user.CompanyId,
                user.RoleId,
                user.Role?.NameEn ?? ""
            );

            return Ok(new
            {
                Token = token,
                UserId = user.Id,
                CompanyId = user.CompanyId,
                RoleId = user.RoleId,
                RoleName = user.Role?.NameEn
            });
        }
    }
}