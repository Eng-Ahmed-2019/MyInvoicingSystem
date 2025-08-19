using InvoicingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<Messages> _localizer;

        public ItemsController(IItemService itemService, IStringLocalizer<Messages> localizer)
        {
            _itemService = itemService;
            _localizer = localizer;
        }

        private bool TryGetCompanyId(out Guid companyId, out ActionResult? errorResult)
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

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<IEnumerable<ItemReadDto>>> GetItems()
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var items = await _itemService.GetItemsAsync(companyId);
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ItemReadDto>> CreateItem(ItemCreateDto dto)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _itemService.CreateItemAsync(companyId, dto);
            if (created == null)
                return Conflict(new { message = _localizer["ItemExists"] ?? "An item with the same name already exists." });

            return CreatedAtAction(nameof(GetItems), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateItem(Guid id, ItemUpdateDto dto)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _itemService.UpdateItemAsync(companyId, id, dto);

            if (result == ItemUpdateStatus.NotFound)
                return NotFound(new { message = _localizer["ItemNotFound"] ?? "Item not found for your company." });

            if (result == ItemUpdateStatus.Conflict)
                return Conflict(new { message = _localizer["AnotherItemExists"] ?? "Another item with the same name already exists." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var deleted = await _itemService.DeleteItemAsync(companyId, id);
            if (!deleted)
                return NotFound(new { message = _localizer["ItemNotFound"] ?? "Item not found for your company." });

            return NoContent();
        }
    }
}