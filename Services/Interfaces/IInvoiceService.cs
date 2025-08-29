using InvoicingSystem.DTOs;

namespace InvoicingSystem.Services.Interfaces
{
    public enum InvoiceItemsCreateStatus
    {
        Success,
        InvoiceNotFound,
        ItemNotFound
    }

    public interface IInvoiceService
    {
        Task<InvoiceReadDto?> CreateInvoiceAsync(Guid companyId, InvoiceCreateDto dto);
        Task<InvoiceItemsCreateStatus> CreateInvoiceItemsAsync(Guid id, List<InvoiceItemCreateDto> itemsDto);
        Task<InvoiceReadDto?> GetInvoiceByIdAsync(Guid companyId, Guid id);
        Task<IEnumerable<InvoiceReadDto>> GetInvoicesAsync(Guid companyId);
    }
}