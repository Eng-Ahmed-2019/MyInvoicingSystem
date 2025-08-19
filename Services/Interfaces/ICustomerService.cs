using InvoicingSystem.DTOs;

namespace InvoicingSystem.Services.Interfaces
{
    public enum CustomerUpdateStatus
    {
        NotFound,
        Conflict,
        Success,
        InvalidData
    }

    public interface ICustomerService
    {
        Task<IEnumerable<CustomerReadDto>> GetCustomersAsync(Guid companyId);
        Task<CustomerReadDto?> CreateCustomerAsync(Guid companyId, CustomerCreateDto dto);
        Task<CustomerUpdateStatus> EditCustomerAsync(Guid companyId, Guid id, CustomerUpdateDto dto);
        Task<bool> DeleteCustomerAsync(Guid companyId, Guid id);
    }
}