using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvoicingSystem.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;

namespace InvoicingSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public ItemsController(ApplicationDbContext context, IStringLocalizer<Messages> localizer)
        {
            _context = context;
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

            var items = await _context.Items
                .Where(i => i.CompanyId == companyId)
                .Select(i => new ItemReadDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    NameAr = i.NameAr,
                    Description = i.Description,
                    DescriptionAr = i.DescriptionAr,
                    UnitPrice = i.UnitPrice
                })
                .ToListAsync();

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

            if (await _context.Items.AnyAsync(i => i.CompanyId == companyId &&
                (i.Name == dto.Name || i.NameAr == dto.NameAr)))
            {
                return Conflict("An item with the same name already exists.");
            }

            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                NameAr = dto.NameAr,
                Description = dto.Description,
                DescriptionAr = dto.DescriptionAr,
                UnitPrice = dto.UnitPrice,
                CompanyId = companyId
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var readDto = new ItemReadDto
            {
                Id = item.Id,
                Name = item.Name,
                NameAr = item.NameAr,
                Description = item.Description,
                DescriptionAr = item.DescriptionAr,
                UnitPrice = item.UnitPrice
            };

            return CreatedAtAction(nameof(GetItems), new { id = item.Id }, readDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateItem(Guid id, ItemUpdateDto dto)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId);

            if (item == null)
                return NotFound("Item not found for your company.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Items.AnyAsync(i => i.CompanyId == companyId &&
                i.Id != id &&
                (i.Name == dto.Name || i.NameAr == dto.NameAr)))
            {
                return Conflict("Another item with the same name already exists.");
            }

            item.Name = dto.Name;
            item.NameAr = dto.NameAr;
            item.Description = dto.Description;
            item.DescriptionAr = dto.DescriptionAr;
            item.UnitPrice = dto.UnitPrice;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId);

            if (item == null)
                return NotFound("Item not found for your company.");

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}