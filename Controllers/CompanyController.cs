using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;

namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public CompanyController(ApplicationDbContext context, IStringLocalizer<Messages> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        [HttpGet("my")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetMyCompany()
        {
            var companyIdObj = HttpContext.Items["CompanyId"];
            if (companyIdObj == null || !Guid.TryParse(companyIdObj.ToString(), out var companyId))
            {
                return BadRequest(new { message = _localizer["InvalidCompanyId"] });
            }

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                return NotFound(new { message = _localizer["CompanyNotFound"] });
            }

            var dto = new CompanyReadDto
            {
                Name = company.Name,
                NameAr = company.NameAr,
                Description = company.Description,
                DescriptionAr = company.DescriptionAr,
            };

            return Ok(dto);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCompanyById(Guid id)
        {
            var companyDto = await _context.Companies
                .Where(c => c.Id == id)
                .Select(c => new CompantDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    NameAr = c.NameAr,
                    Description = c.Description!,
                    DescriptionAr = c.DescriptionAr!
                })
                .FirstOrDefaultAsync();

            if (companyDto == null)
            {
                return NotFound(new { message = _localizer["CompanyNotFound"] });
            }
            return Ok(companyDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Companies.AnyAsync(c => c.Name == dto.Name || c.NameAr == dto.NameAr))
            {
                return Conflict(new { message = "Company with the same name already exists." });
            }

            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                NameAr = dto.NameAr,
                Description = dto.Description,
                DescriptionAr = dto.DescriptionAr,
                CreatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            var resultDto = new CompanyResultDto
            {
                Id = company.Id,
                Name = company.Name,
                NameAr = company.NameAr,
                Description = company.Description!,
                DescriptionAr = company.DescriptionAr!
            };

            return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, resultDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return NotFound(new { message = _localizer["CompanyNotFound"] });

            company.Name = dto.Name;
            company.NameAr = dto.NameAr;
            company.Description = dto.Description;
            company.DescriptionAr = dto.DescriptionAr;
            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var resultDto = new CompanyResultDto
            {
                Id = company.Id,
                Name = company.Name,
                NameAr = company.NameAr,
                Description = company.Description!,
                DescriptionAr = company.DescriptionAr!
            };

            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound(new { message = _localizer["CompanyNotFound"] });
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}