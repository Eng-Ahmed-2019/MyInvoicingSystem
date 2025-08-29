using InvoicingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Services.Interfaces;
using InvoicingSystem.Services;

namespace InvoicingSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly FileLoggerService _fileLoggerService;

        public ItemsController(IItemService itemService, IStringLocalizer<Messages> localizer, FileLoggerService fileLoggerService)
        {
            _itemService = itemService;
            _localizer = localizer;
            _fileLoggerService = fileLoggerService;
        }

        private bool TryGetCompanyId(out Guid companyId, out ActionResult? errorResult)
        {
            _fileLoggerService.Log("Extracting CompanyId from HttpContext.Items");

            try
            {
                companyId = Guid.Empty;
                errorResult = null;

                var companyIdObj = HttpContext.Items["CompanyId"];
                if (companyIdObj == null || !Guid.TryParse(companyIdObj.ToString(), out companyId))
                {
                    errorResult = BadRequest(new { message = _localizer["InvalidCompanyId"] });
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                companyId = Guid.Empty;
                errorResult = StatusCode(500, new { message = _localizer["ServerError"] });
                return false;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<IEnumerable<ItemReadDto>>> GetItems()
        {
            _fileLoggerService.Log("GetItems endpoint called");

            try
            {
                if (!TryGetCompanyId(out var companyId, out var errorResult))
                    return errorResult!;

                var items = await _itemService.GetItemsAsync(companyId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ItemReadDto>> CreateItem(ItemCreateDto dto)
        {
            _fileLoggerService.Log("CreateItem endpoint called");

            try
            {
                if (!TryGetCompanyId(out var companyId, out var errorResult))
                    return errorResult!;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _itemService.CreateItemAsync(companyId, dto);
                if (created == null)
                    return Conflict(new { message = _localizer["ItemExists"] });

                return CreatedAtAction(nameof(GetItems), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"]});
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateItem(Guid id, ItemUpdateDto dto)
        {
            _fileLoggerService.Log("UpdateItem endpoint called");

            try
            {
                if (!TryGetCompanyId(out var companyId, out var errorResult))
                    return errorResult!;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _itemService.UpdateItemAsync(companyId, id, dto);

                if (result == ItemUpdateStatus.NotFound)
                    return NotFound(new { message = _localizer["ItemNotFound"] });

                if (result == ItemUpdateStatus.Conflict)
                    return Conflict(new { message = _localizer["AnotherItemExists"] });

                return NoContent();
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            _fileLoggerService.Log("DeleteItem endpoint called");

            try
            {
                if (!TryGetCompanyId(out var companyId, out var errorResult))
                    return errorResult!;

                var deleted = await _itemService.DeleteItemAsync(companyId, id);
                if (!deleted)
                    return NotFound(new { message = _localizer["ItemNotFound"] });

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