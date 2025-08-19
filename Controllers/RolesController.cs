using InvoicingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        private bool TryGetCompanyId(out Guid companyId)
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

        [Authorize(Roles = "Admin,Manager,User")]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetMyRoles()
        {
            if (!TryGetCompanyId(out var companyId))
                return BadRequest("Company ID is missing or invalid.");

            var roles = await _roleService.GetRolesAsync(companyId);
            return Ok(roles);
        }

        [Authorize(Roles = "Admin,Manager,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(Guid id)
        {
            if (!TryGetCompanyId(out var companyId))
                return BadRequest("Company ID is missing or invalid.");

            var roleDto = await _roleService.GetRoleByIdAsync(companyId, id);
            if (roleDto == null)
                return NotFound("Role not found or does not belong to your company.");

            return Ok(roleDto);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleDto roleDto)
        {
            if (!TryGetCompanyId(out var companyId))
                return BadRequest("Company ID is missing or invalid.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRole = await _roleService.CreateRoleAsync(companyId, roleDto);
            return CreatedAtAction(nameof(GetRole), new { id = createdRole!.Id }, createdRole);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, RoleDto roleDto)
        {
            if (!TryGetCompanyId(out var companyId))
                return BadRequest("Company ID is missing or invalid.");

            if (id != roleDto.Id)
                return BadRequest("Role ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _roleService.UpdateRoleAsync(companyId, id, roleDto);
            if (!updated)
                return NotFound("Role not found or does not belong to your company.");

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            if (!TryGetCompanyId(out var companyId))
                return BadRequest("Company ID is missing or invalid.");

            var deleted = await _roleService.DeleteRoleAsync(companyId, id);
            if (!deleted)
                return NotFound("Role not found or does not belong to your company.");

            return NoContent();
        }
    }
}