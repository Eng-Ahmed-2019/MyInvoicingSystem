using InvoicingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Services.Interfaces;
using InvoicingSystem.Services;

namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly FileLoggerService _fileLoggerService;

        public CompanyController(ICompanyService companyService, IStringLocalizer<Messages> localizer, FileLoggerService fileLoggerService)
        {
            _companyService = companyService;
            _localizer = localizer;
            _fileLoggerService = fileLoggerService;
        }

        [HttpGet("my")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetMyCompany()
        {
            _fileLoggerService.Log("GetMyCompany endpoint called.");

            try
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
            catch(Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCompanyById(Guid id)
        {
            _fileLoggerService.Log($"GetCompanyById endpoint called with id: {id}");

            try
            {
                var companyDto = await _companyService.GetCompanyByIdAsync(id);

                if (companyDto == null)
                {
                    return NotFound(new { message = _localizer["CompanyNotFound"] });
                }
                return Ok(companyDto);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDto dto)
        {
            _fileLoggerService.Log("CreateCompany endpoint called.");

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _companyService.CreateCompanyAsync(dto);
                if (result == null)
                    return Conflict(new { message = _localizer["CompanyAlreadyExists"] });

                return CreatedAtAction(nameof(GetCompanyById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyUpdateDto dto)
        {
            _fileLoggerService.Log($"UpdateCompany endpoint called with id: {id}");

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _companyService.UpdateCompanyAsync(id, dto);
                if (result == null)
                    return NotFound(new { message = _localizer["CompanyNotFound"] });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            _fileLoggerService.Log($"DeleteCompany endpoint called with id: {id}");

            try
            {
                var deleted = await _companyService.DeleteCompanyAsync(id);
                if (!deleted)
                    return NotFound(new { message = _localizer["CompanyNotFound"] });

                return NoContent();
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }
    }
}