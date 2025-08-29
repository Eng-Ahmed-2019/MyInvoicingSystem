using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.EntityFrameworkCore;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceReadDto?> CreateInvoiceAsync(Guid companyId, InvoiceCreateDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CompanyId == companyId);
            if (customer == null)
                return null;

            decimal subTotal = 0m;
            decimal tax = dto.Tax ?? 0m;
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

            return new InvoiceReadDto
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
                CreatedAt = invoice.CreatedAt,
                Items = new List<InvoiceItemDto>()
            };
        }

        public async Task<InvoiceItemsCreateStatus> CreateInvoiceItemsAsync(Guid id, List<InvoiceItemCreateDto> itemsDto)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return InvoiceItemsCreateStatus.InvoiceNotFound;

            decimal addedSubtotal = 0m;
            foreach (var itemDto in itemsDto)
            {
                var existingItem = await _context.Items.FindAsync(itemDto.ItemId);
                if (existingItem == null)
                {
                    return InvoiceItemsCreateStatus.ItemNotFound;
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
                    Total = itemDto.Quantity * itemDto.UnitPrice,
                    CreatedAt = DateTime.UtcNow
                };

                _context.InvoiceItems.Add(invoiceItem);
                addedSubtotal += invoiceItem.Total;
            }

            invoice.Subtotal += addedSubtotal;
            invoice.Total = invoice.Subtotal + (invoice.Tax ?? 0m);

            await _context.SaveChangesAsync();

            return InvoiceItemsCreateStatus.Success;
        }

        public async Task<InvoiceReadDto?> GetInvoiceByIdAsync(Guid companyId, Guid id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId);

            if (invoice == null) return null;

            return new InvoiceReadDto
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
                CreatedAt = invoice.CreatedAt,
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
        }

        public async Task<IEnumerable<InvoiceReadDto>> GetInvoicesAsync(Guid companyId)
        {
            return await _context.Invoices
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
                    CreatedAt = i.CreatedAt,
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
        }
    }
}