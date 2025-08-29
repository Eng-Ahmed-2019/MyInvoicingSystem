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
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly FileLoggerService _fileLoggerService;

        public InvoicesController(IInvoiceService invoiceService, IStringLocalizer<Messages> localizer, FileLoggerService fileLoggerService)
        {
            _invoiceService = invoiceService;
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

        [HttpPost]
        [Authorize(Policy = "CreateInvoice")]
        public async Task<ActionResult<InvoiceReadDto>> CreateInvoice(InvoiceCreateDto dto)
        {
            _fileLoggerService.Log("CreateInvoice endpoint called");

            try
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
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpPost("{invoiceId}/items")]
        [Authorize(Policy = "CreateInvoiceItems")]
        public async Task<ActionResult> CreateInvoiceItems(string invoiceNumber, List<InvoiceItemCreateDto> itemsDto)
        {
            _fileLoggerService.Log("CreateInvoiceItems endpoint called");

            try
            {
                if (itemsDto == null || !itemsDto.Any())
                    return BadRequest(new { message = _localizer["InvalidRequest"] });

                var status = await _invoiceService.CreateInvoiceItemsAsync(invoiceNumber, itemsDto);

                if (status == InvoiceItemsCreateStatus.InvoiceNotFound)
                    return NotFound(new { message = _localizer["InvoiceNotFound"] });

                if (status == InvoiceItemsCreateStatus.ItemNotFound)
                    return BadRequest(new { message = _localizer["InvoiceItemNotFound"] });

                return Ok(new { message = _localizer["InvoiceItemsCreated"] });
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ViewInvoice")]
        public async Task<ActionResult<InvoiceReadDto>> GetInvoice(Guid id)
        {
            _fileLoggerService.Log("GetInvoice endpoint called");

            try
            {
                if (!TryGetCompanyId(out var companyId, out var errorResult))
                    return errorResult!;

                var invoice = await _invoiceService.GetInvoiceByIdAsync(companyId, id);

                if (invoice == null)
                    return NotFound(new { message = _localizer["InvoiceNotFound"] });

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }

        [HttpGet]
        [Authorize(Policy = "ViewInvoice")]
        public async Task<ActionResult<IEnumerable<InvoiceReadDto>>> GetInvoices()
        {
            _fileLoggerService.Log("GetInvoices endpoint called");

            try
            {
                if (!TryGetCompanyId(out var companyId, out var errorResult))
                    return errorResult!;

                var invoices = await _invoiceService.GetInvoicesAsync(companyId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _fileLoggerService.LogError(ex);
                return StatusCode(500, new { message = _localizer["ServerError"] });
            }
        }
    }
}