using InvoicingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Services.Interfaces;
using InvoicingSystem.Services;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;

namespace InvoicingSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly FileLoggerService _fileLoggerService;
        private readonly IStringLocalizer<Messages> _localizer;

        public RolesController(IRoleService roleService, FileLoggerService fileLoggerService, IStringLocalizer<Messages> stringLocalizer)
        {
            _roleService = roleService;
            _fileLoggerService = fileLoggerService;
            _localizer = stringLocalizer;
        }

        private bool TryGetCompanyId(out Guid companyId)
        {
            _fileLoggerService.Log("Attempting to retrieve CompanyId from claims, headers, or HttpContext items.");

            try
            {
                companyId = Guid.Empty;

                var companyIdClaim = User.FindFirst("CompanyId")?.Value;
                if (!string.IsNullOrEmpty(companyIdClaim) && Guid.TryParse(companyIdClaim, out companyId))
                    return true;

                if (Request.Headers.TryGetValue("X-Company-Id", out var companyIdHeader) &&
                    Guid.TryParse(companyIdHeader, out companyId))
                    return true;

                var companyIdString = HttpContext.Items["CompanyId"]?.ToString();
                if (!string.IsNullOrEmpty(companyIdString) && Guid.TryParse(companyIdString, out companyId))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                _fileLoggerService.Log($"Error retrieving CompanyId: {ex.Message}");
                companyId = Guid.Empty;
                return false;
            }
        }

        [Authorize(Roles = "Admin,Manager,User")]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetMyRoles()
        {
            _fileLoggerService.Log("GetMyRoles endpoint called.");

            try
            {
                if (!TryGetCompanyId(out var companyId))
                    return BadRequest(_localizer["Company ID is missing"]);

                var roles = await _roleService.GetRolesAsync(companyId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _fileLoggerService.Log($"Error in GetMyRoles: {ex.Message}");
                return StatusCode(500, _localizer["ServerError"]);
            }
        }

        [Authorize(Roles = "Admin,Manager,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(Guid id)
        {
            _fileLoggerService.Log($"GetRole endpoint called with ID: {id}");

            try
            {
                if (!TryGetCompanyId(out var companyId))
                    return BadRequest(_localizer["Company ID is missing"]);

                var roleDto = await _roleService.GetRoleByIdAsync(companyId, id);
                if (roleDto == null)
                    return NotFound(_localizer["Role not found"]);

                return Ok(roleDto);
            }
            catch (Exception ex)
            {
                _fileLoggerService.Log($"Error in GetRole: {ex.Message}");
                return StatusCode(500, _localizer["ServerError"]);
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleDto roleDto)
        {
            _fileLoggerService.Log("CreateRole endpoint called.");

            try
            {
                if (!TryGetCompanyId(out var companyId))
                    return BadRequest(_localizer["Company ID is missing"]);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdRole = await _roleService.CreateRoleAsync(companyId, roleDto);
                return CreatedAtAction(nameof(GetRole), new { id = createdRole!.Id }, createdRole);
            }
            catch (Exception ex)
            {
                _fileLoggerService.Log($"Error in CreateRole: {ex.Message}");
                return StatusCode(500, _localizer["ServerError"]);
            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, RoleDto roleDto)
        {
            _fileLoggerService.Log($"UpdateRole endpoint called with ID: {id}");

            try
            {
                if (!TryGetCompanyId(out var companyId))
                    return BadRequest(_localizer["Company ID is missing"]);

                if (id != roleDto.Id)
                    return BadRequest("Role ID mismatch.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _roleService.UpdateRoleAsync(companyId, id, roleDto);
                if (!updated)
                    return NotFound(_localizer["Role not found"]);

                return NoContent();
            }
            catch (Exception ex)
            {
                _fileLoggerService.Log($"Error in UpdateRole: {ex.Message}");
                return StatusCode(500, _localizer["ServerError"]);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            _fileLoggerService.Log($"DeleteRole endpoint called with ID: {id}");

            try
            {
                if (!TryGetCompanyId(out var companyId))
                    return BadRequest(_localizer["Company ID is missing"]);

                var deleted = await _roleService.DeleteRoleAsync(companyId, id);
                if (!deleted)
                    return NotFound(_localizer["Role not found"]);

                return NoContent();
            }
            catch (Exception ex)
            {
                _fileLoggerService.Log($"Error in DeleteRole: {ex.Message}");
                return StatusCode(500, _localizer["ServerError"]);
            }
        }
    }
}