using InvoicingSystem.Data;
using InvoicingSystem.DTOs;
using InvoicingSystem.Models;
using Microsoft.EntityFrameworkCore;
using InvoicingSystem.Services.Interfaces;

namespace InvoicingSystem.Services.Implementations
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        public ItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemReadDto>> GetItemsAsync(Guid companyId)
        {
            return await _context.Items
                .AsNoTracking()
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
        }

        public async Task<ItemReadDto?> GetItemByIdAsync(Guid companyId, Guid id)
        {
            var i = await _context.Items
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId);

            if (i == null) return null;

            return new ItemReadDto
            {
                Id = i.Id,
                Name = i.Name,
                NameAr = i.NameAr,
                Description = i.Description,
                DescriptionAr = i.DescriptionAr,
                UnitPrice = i.UnitPrice
            };
        }

        public async Task<ItemReadDto?> CreateItemAsync(Guid companyId, ItemCreateDto dto)
        {
            var exists = await _context.Items.AnyAsync(i =>
                i.CompanyId == companyId && (i.Name == dto.Name || i.NameAr == dto.NameAr));

            if (exists) return null;

            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                NameAr = dto.NameAr,
                Description = dto.Description,
                DescriptionAr = dto.DescriptionAr,
                UnitPrice = dto.UnitPrice,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return new ItemReadDto
            {
                Id = item.Id,
                Name = item.Name,
                NameAr = item.NameAr,
                Description = item.Description,
                DescriptionAr = item.DescriptionAr,
                UnitPrice = item.UnitPrice
            };
        }

        public async Task<ItemUpdateStatus> UpdateItemAsync(Guid companyId, Guid id, ItemUpdateDto dto)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId);
            if (item == null) return ItemUpdateStatus.NotFound;

            var conflict = await _context.Items.AnyAsync(i =>
                i.CompanyId == companyId && i.Id != id && (i.Name == dto.Name || i.NameAr == dto.NameAr));
            if (conflict) return ItemUpdateStatus.Conflict;

            item.Name = dto.Name;
            item.NameAr = dto.NameAr;
            item.Description = dto.Description;
            item.DescriptionAr = dto.DescriptionAr;
            item.UnitPrice = dto.UnitPrice;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ItemUpdateStatus.Success;
        }

        public async Task<bool> DeleteItemAsync(Guid companyId, Guid id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId);
            if (item == null) return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}