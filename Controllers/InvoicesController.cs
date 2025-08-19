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
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IStringLocalizer<Messages> _localizer;

        public InvoicesController(IInvoiceService invoiceService, IStringLocalizer<Messages> localizer)
        {
            _invoiceService = invoiceService;
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
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _invoiceService.CreateInvoiceAsync(companyId, dto);
            if (created == null)
            {
                return BadRequest(new { message = _localizer["CustomerNotFound"] });
            }

            return CreatedAtAction(nameof(GetInvoice), new { id = created.Id }, created);
        }

        [HttpPost("{invoiceId}/items")]
        [Authorize(Policy = "CreateInvoiceItems")]
        public async Task<ActionResult> CreateInvoiceItems(string invoiceNumber, List<InvoiceItemCreateDto> itemsDto)
        {
            if (itemsDto == null || !itemsDto.Any())
                return BadRequest(new { message = _localizer["InvalidRequest"] ?? "Invalid items payload." });

            var status = await _invoiceService.CreateInvoiceItemsAsync(invoiceNumber, itemsDto);

            if (status == InvoiceItemsCreateStatus.InvoiceNotFound)
                return NotFound(new { message = _localizer["InvoiceNotFound"] });

            if (status == InvoiceItemsCreateStatus.ItemNotFound)
                return BadRequest(new { message = _localizer["InvoiceItemNotFound"] ?? "One or more items not found." });

            return Ok(new { message = _localizer["InvoiceItemsCreated"] ?? "Invoice items created successfully" });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ViewInvoice")]
        public async Task<ActionResult<InvoiceReadDto>> GetInvoice(Guid id)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var invoice = await _invoiceService.GetInvoiceByIdAsync(companyId, id);

            if (invoice == null)
                return NotFound(new { message = _localizer["InvoiceNotFound"] });

            return Ok(invoice);
        }

        [HttpGet]
        [Authorize(Policy = "ViewInvoice")]
        public async Task<ActionResult<IEnumerable<InvoiceReadDto>>> GetInvoices()
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var invoices = await _invoiceService.GetInvoicesAsync(companyId);
            return Ok(invoices);
        }
    }
}