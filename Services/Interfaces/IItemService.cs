using InvoicingSystem.DTOs;

namespace InvoicingSystem.Services.Interfaces
{
    public enum ItemUpdateStatus
    {
        NotFound,
        Conflict,
        Success
    }

    public interface IItemService
    {
        Task<IEnumerable<ItemReadDto>> GetItemsAsync(Guid companyId);
        Task<ItemReadDto?> GetItemByIdAsync(Guid companyId, Guid id);
        Task<ItemReadDto?> CreateItemAsync(Guid companyId, ItemCreateDto dto);
        Task<ItemUpdateStatus> UpdateItemAsync(Guid companyId, Guid id, ItemUpdateDto dto);
        Task<bool> DeleteItemAsync(Guid companyId, Guid id);
    }
}