using InvoicingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Services.Interfaces;
using InvoicingSystem.Services;

namespace InvoicingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly FileLoggerService _fileLoggerService;

        public UsersController(IUserService userService, IStringLocalizer<Messages> localizer, FileLoggerService fileLoggerService)
        {
            _userService = userService;
            _localizer = localizer;
            _fileLoggerService = fileLoggerService;
        }

        private (bool isValid, Guid companyId) GetCompanyId()
        {
            _fileLoggerService.Log("Retrieving CompanyId");

            try
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
            catch (Exception ex)
            {
                _fileLoggerService.Log($"Error retrieving CompanyId: {ex.Message}");
                return (false, Guid.Empty);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            _fileLoggerService.Log("GetUsers endpoint called");

            try
            {
                var (isValid, companyId) = GetCompanyId();
                if (!isValid) return BadRequest(_localizer["CompanyIdMissing"]);

                var users = await _userService.GetUsersAsync(companyId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _fileLoggerService.Log($"Error in GetUsers: {ex.Message}");
                return StatusCode(500, _localizer["ServerError"]);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadDto>> GetUserById(Guid id)
        {
            _fileLoggerService.Log("GetUserById endpoint called");

            try
            {
                var (isValid, companyId) = GetCompanyId();
                if (!isValid) return BadRequest(_localizer["CompanyIdMissing"]);

                var user = await _userService.GetUserByIdAsync(companyId, id);
                if (user == null)
                    return NotFound(_localizer["UserNotFound"]);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadDto>> CreateUser(UserCreateDto dto)
        {
            _fileLoggerService.Log("CreateUser endpoint called");

            try
            {
                var (isValid, companyId) = GetCompanyId();
                if (!isValid) return BadRequest(_localizer["CompanyIdMissing"]);

                if (await Task.FromResult(!ModelState.IsValid))
                    return BadRequest(ModelState);

                var created = await _userService.CreateUserAsync(companyId, dto);
                if (created == null)
                    return Conflict(_localizer["UserExists"]);

                return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto dto)
        {
            _fileLoggerService.Log("UpdateUser endpoint called");

            try
            {
                var (isValid, companyId) = GetCompanyId();
                if (!isValid) return BadRequest(_localizer["CompanyIdMissing"]);

                if (await Task.FromResult(!ModelState.IsValid))
                    return BadRequest(ModelState);

                var (success, conflict) = await _userService.UpdateUserAsync(companyId, id, dto);

                if (!success && !conflict)
                    return NotFound(_localizer["UserNotFound"]);

                if (conflict)
                    return Conflict(_localizer["UserConflict"]);

                return NoContent();
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            _fileLoggerService.Log("DeleteUser endpoint called");

            try
            {
                var (isValid, companyId) = GetCompanyId();
                if (!isValid) return BadRequest(_localizer["CompanyIdMissing"]);

                var deleted = await _userService.DeleteUserAsync(companyId, id);
                if (!deleted)
                    return NotFound(_localizer["UserNotFound"]);

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