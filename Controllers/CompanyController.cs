using InvoicingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<Messages> _localizer;

        public CompanyController(ICompanyService companyService, IStringLocalizer<Messages> localizer)
        {
            _companyService = companyService;
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

            var company = await _companyService.GetMyCompanyAsync(companyId);

            if (company == null)
            {
                return NotFound(new { message = _localizer["CompanyNotFound"] });
            }

            return Ok(company);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCompanyById(Guid id)
        {
            var companyDto = await _companyService.GetCompanyByIdAsync(id);

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

            var result = await _companyService.CreateCompanyAsync(dto);
            if (result == null)
                return Conflict(new { message = _localizer["CompanyAlreadyExists"] ?? "Company with the same name already exists." });

            return CreatedAtAction(nameof(GetCompanyById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _companyService.UpdateCompanyAsync(id, dto);
            if (result == null)
                return NotFound(new { message = _localizer["CompanyNotFound"] });

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var deleted = await _companyService.DeleteCompanyAsync(id);
            if (!deleted)
                return NotFound(new { message = _localizer["CompanyNotFound"] });

            return NoContent();
        }
    }
}