using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using InvoicingSystem.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;

namespace InvoicingSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public InvoicesController(ApplicationDbContext context, IStringLocalizer<Messages> localizer)
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

        [HttpPost]
        [Authorize(Policy = "CreateInvoice")]
        public async Task<ActionResult<InvoiceReadDto>> CreateInvoice(InvoiceCreateDto dto)
        {
            bool hasUsers = await _context.Customers.AnyAsync();
            if (!hasUsers)
            {
                return BadRequest(new { message = "لا يمكن إنشاء فاتورة قبل تسجيل مستخدم واحد على الأقل." });
            }

            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CompanyId == companyId);
            if (customer == null)
                return BadRequest(new { message = _localizer["CustomerNotFound"] });

            decimal subTotal = 0;
            decimal tax = dto.Tax ?? 0;
            decimal total = subTotal + tax;

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = dto.InvoiceNumber,
                Title = dto.Title,
                TitleAr = dto.TitleAr,
                Description = dto.Description,
                DescriptionAr = dto.DescriptionAr,
                CustomerId = customer.Id,
                CompanyId = companyId,
                Subtotal = subTotal,
                Tax = tax,
                Total = total,
                CreatedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            var readDto = new InvoiceReadDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Title = invoice.Title,
                TitleAr = invoice.TitleAr,
                Description = invoice.Description,
                DescriptionAr = invoice.DescriptionAr,
                Subtotal = invoice.Subtotal,
                Tax = invoice.Tax,
                Total = invoice.Total
                //Items = new List<InvoiceItemDto>()
            };

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, readDto);
        }
        [HttpPost("{invoiceId}/items")]
        [Authorize(Policy = "CreateInvoiceItems")]
        public async Task<ActionResult> CreateInvoiceItems(string invoiceNumber, List<InvoiceItemCreateDto> itemsDto)
        {
            bool hasInvoice = await _context.Invoices.AnyAsync();
            if (!hasInvoice)
            {
                return BadRequest(new { message = "لا يمكن إنشاء عناصر قبل تسجيل فاتورة واحدة على الأقل." });
            }

            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
            if (invoice == null)
                return NotFound(new { message = "الفاتورة غير موجودة." });

            foreach (var itemDto in itemsDto)
            {
                var existingItem = await _context.Items.FindAsync(itemDto.ItemId);
                if (existingItem == null)
                {
                    return BadRequest(new { message = $"العنصر بالمعرّف {itemDto.ItemId} غير موجود." });
                }
                var invoiceItem = new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    ItemId = itemDto.ItemId,
                    Name = itemDto.Name,
                    NameAr = itemDto.NameAr,
                    Description = itemDto.Description,
                    DescriptionAr = itemDto.DescriptionAr,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Total = itemDto.Quantity * itemDto.UnitPrice
                };
                _context.InvoiceItems.Add(invoiceItem);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Invoice items created successfully" });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ViewInvoice")]
        public async Task<ActionResult<InvoiceReadDto>> GetInvoice(Guid id)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Company)
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId);

            if (invoice == null)
                return NotFound(new { message = _localizer["InvoiceNotFound"] });

            var readDto = new InvoiceReadDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                Title = invoice.Title,
                TitleAr = invoice.TitleAr,
                Description = invoice.Description,
                DescriptionAr = invoice.DescriptionAr,
                Subtotal = invoice.Subtotal,
                Tax = invoice.Tax,
                Total = invoice.Total,
                Items = invoice.InvoiceItems.Select(it => new InvoiceItemDto
                {
                    Name = it.Name,
                    NameAr = it.NameAr,
                    Description = it.Description,
                    DescriptionAr = it.DescriptionAr,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice
                }).ToList()
            };

            return Ok(readDto);
        }

        [HttpGet]
        [Authorize(Policy = "ViewInvoice")]
        public async Task<ActionResult<IEnumerable<InvoiceReadDto>>> GetInvoices()
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var invoices = await _context.Invoices
                .Where(i => i.CompanyId == companyId)
                .Include(i => i.InvoiceItems)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new InvoiceReadDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    Title = i.Title,
                    TitleAr = i.TitleAr,
                    Description = i.Description,
                    DescriptionAr = i.DescriptionAr,
                    Subtotal = i.Subtotal,
                    Tax = i.Tax,
                    Total = i.Total,
                    Items = i.InvoiceItems.Select(it => new InvoiceItemDto
                    {
                        Name = it.Name,
                        NameAr = it.NameAr,
                        Description = it.Description,
                        DescriptionAr = it.DescriptionAr,
                        Quantity = it.Quantity,
                        UnitPrice = it.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return Ok(invoices);
        }
    }
}