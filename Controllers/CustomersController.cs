using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using InvoicingSystem.Localization;
using Microsoft.Extensions.Localization;

namespace InvoicingSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public CustomersController(ApplicationDbContext context, IStringLocalizer<Messages> localizer)
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<CustomerReadDto>>> GetCustomers()
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var customers = await _context.Customers
                .Where(c => c.CompanyId == companyId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CustomerReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    NameAr = c.NameAr,
                    Email = c.Email,
                    Phone = c.Phone,
                })
                .ToListAsync();

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

            if (await _context.Customers.AnyAsync(c => c.CompanyId == companyId &&
                ((dto.Email != null && c.Email == dto.Email) ||
                 (dto.Phone != null && c.Phone == dto.Phone))))
            {
                return Conflict(new { message = _localizer["CustomerExists"] });
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                NameAr = dto.NameAr,
                Email = dto.Email,
                Phone = dto.Phone,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var readDto = new CustomerReadDto
            {
                Id = customer.Id,
                Name = customer.Name,
                NameAr = customer.NameAr,
                Email = customer.Email,
                Phone = customer.Phone,
            };

            return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, readDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(Guid id, CustomerUpdateDto dto)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (customer == null)
                return NotFound(new { message = _localizer["CustomerNotFound"] });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Customers.AnyAsync(c => c.CompanyId == companyId &&
                c.Id != id &&
                ((dto.Email != null && c.Email == dto.Email) ||
                 (dto.Phone != null && c.Phone == dto.Phone))))
            {
                return Conflict(new { message = _localizer["CustomerExists"] });
            }

            customer.Name = dto.Name;
            customer.NameAr = dto.NameAr;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            if (!TryGetCompanyId(out var companyId, out var errorResult))
                return errorResult!;

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == companyId);

            if (customer == null)
                return NotFound(new { message = _localizer["CustomerNotFound"] });

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}