using InvoicingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<Messages> _localizer;

        public UsersController(IUserService userService, IStringLocalizer<Messages> localizer)
        {
            _userService = userService;
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
            if (!isValid) return BadRequest(_localizer["CompanyIdMissing"] ?? "Company ID is missing or invalid.");

            var users = await _userService.GetUsersAsync(companyId);
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadDto>> GetUserById(Guid id)
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest(_localizer["CompanyIdMissing"] ?? "Company ID is missing or invalid.");

            var user = await _userService.GetUserByIdAsync(companyId, id);
            if (user == null)
                return NotFound(_localizer["UserNotFound"] ?? "User not found or does not belong to your company.");

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadDto>> CreateUser(UserCreateDto dto)
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest(_localizer["CompanyIdMissing"] ?? "Company ID is missing or invalid.");

            if (await Task.FromResult(!ModelState.IsValid))
                return BadRequest(ModelState);

            var created = await _userService.CreateUserAsync(companyId, dto);
            if (created == null)
                return Conflict(_localizer["UserExists"] ?? "A user with the same email or username already exists in this company.");

            return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto dto)
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest(_localizer["CompanyIdMissing"] ?? "Company ID is missing or invalid.");

            if (await Task.FromResult(!ModelState.IsValid))
                return BadRequest(ModelState);

            var (success, conflict) = await _userService.UpdateUserAsync(companyId, id, dto);

            if (!success && !conflict)
                return NotFound(_localizer["UserNotFound"] ?? "User not found or does not belong to your company.");

            if (conflict)
                return Conflict(_localizer["UserConflict"] ?? "Another user with the same email or username already exists.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var (isValid, companyId) = GetCompanyId();
            if (!isValid) return BadRequest(_localizer["CompanyIdMissing"] ?? "Company ID is missing or invalid.");

            var deleted = await _userService.DeleteUserAsync(companyId, id);
            if (!deleted)
                return NotFound(_localizer["UserNotFound"] ?? "User not found or does not belong to your company.");

            return NoContent();
        }
    }
}