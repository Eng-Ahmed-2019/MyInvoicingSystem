using InvoicingSystem.Data;
using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;
using InvoicingSystem.DTOs;

namespace InvoicingSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public RolesController(ApplicationDbContext context, IStringLocalizer<Messages> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        private bool TryGetCompanyId(out Guid companyId)
        {
            companyId = Guid.Empty;

            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyIdClaim) && Guid.TryParse(companyIdClaim, out companyId))
            {
                return true;
            }

            if (Request.Headers.TryGetValue("X-Company-Id", out var companyIdHeader) &&
                Guid.TryParse(companyIdHeader, out companyId))
            {
                return true;
            }

            var companyIdString = HttpContext.Items["CompanyId"]?.ToString();
            if (!string.IsNullOrEmpty(companyIdString) && Guid.TryParse(companyIdString, out companyId))
            {
                return true;
            }

            return false;
        }

        [Authorize(Roles = "Admin,Manager,User")]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<Role>>> GetMyRoles()
        {
            if (!TryGetCompanyId(out var companyId))
            {
                return BadRequest("Company ID is missing or invalid.");
            }

            var roles = await _context.Roles
                .Where(r => r.CompanyId == companyId)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.NameEn,
                    NameAr = r.NameAr,
                    Description = r.DescriptionEn!,
                    DescriptionAr = r.DescriptionAr!,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(roles);
        }

        [Authorize(Roles = "Admin,Manager,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(Guid id)
        {
            if (!TryGetCompanyId(out var companyId))
            {
                return BadRequest("Company ID is missing or invalid.");
            }

            var roleDto = await _context.Roles
                .Where(r => r.Id == id && r.CompanyId == companyId)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.NameEn,
                    NameAr = r.NameAr,
                    Description = r.DescriptionEn!,
                    DescriptionAr = r.DescriptionAr!
                })
                .FirstOrDefaultAsync();

            if (roleDto == null)
            {
                return NotFound("Role not found or does not belong to your company.");
            }

            return Ok(roleDto);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole(RoleDto roleDto)
        {
            if (!TryGetCompanyId(out var companyId))
            {
                return BadRequest("Company ID is missing or invalid.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            var resultDto = new RoleDto
            {
                Id = role.Id,
                Name = role.NameAr,
                NameAr = role.NameAr,
                Description = role.DescriptionEn ?? string.Empty,
                DescriptionAr = role.DescriptionAr ?? string.Empty,
                CreatedAt = role.CreatedAt
            };

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, resultDto);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, RoleDto roleDto)
        {
            if (!TryGetCompanyId(out var companyId))
            {
                return BadRequest("Company ID is missing or invalid.");
            }

            if (id != roleDto.Id)
            {
                return BadRequest("Role ID mismatch.");
            }

            var exRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == companyId);
            if (exRole == null)
            {
                return NotFound("Role not found or does not belong to your company.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            exRole.NameEn = roleDto.Name;
            exRole.NameAr = roleDto.NameAr;
            exRole.DescriptionEn = roleDto.Description;
            exRole.DescriptionAr = roleDto.DescriptionAr;
            exRole.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            if (!TryGetCompanyId(out var companyId))
            {
                return BadRequest("Company ID is missing or invalid.");
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id && r.CompanyId == companyId);

            if (role == null)
            {
                return NotFound("Role not found or does not belong to your company.");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}