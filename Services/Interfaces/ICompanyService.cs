using InvoicingSystem.DTOs;

namespace InvoicingSystem.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyReadDto?> GetMyCompanyAsync(Guid companyId);
        Task<CompantDTO?> GetCompanyByIdAsync(Guid id);
        Task<CompanyResultDto?> CreateCompanyAsync(CompanyCreateDto dto);
        Task<CompanyResultDto?> UpdateCompanyAsync(Guid id, CompanyUpdateDto dto);
        Task<bool> DeleteCompanyAsync(Guid id);
    }
}