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
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IStringLocalizer<Messages> _localizer;

        public CustomersController(ICustomerService customerService, IStringLocalizer<Messages> localizer)
        {
            _customerService = customerService;
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<CustomerReadDto>>> GetCustomers()
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var customers = await _customerService.GetCustomersAsync(companyId);
            return Ok(customers);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<CustomerReadDto>> CreateCustomer(CustomerCreateDto dto)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _customerService.CreateCustomerAsync(companyId, dto);
            if (created == null)
                return Conflict(new { message = _localizer["CustomerExists"] });

            return CreatedAtAction(nameof(GetCustomers), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(Guid id, CustomerUpdateDto dto)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerService.EditCustomerAsync(companyId, id, dto);

            if (result == CustomerUpdateStatus.NotFound)
                return NotFound(new { message = _localizer["CustomerNotFound"] });

            if (result == CustomerUpdateStatus.Conflict)
                return Conflict(new { message = _localizer["CustomerExists"] });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var deleted = await _customerService.DeleteCustomerAsync(companyId, id);
            if (!deleted)
                return NotFound(new { message = _localizer["CustomerNotFound"] });

            return NoContent();
        }
    }
}