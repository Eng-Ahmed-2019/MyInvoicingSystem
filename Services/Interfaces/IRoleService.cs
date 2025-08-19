using InvoicingSystem.DTOs;

namespace InvoicingSystem.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetRolesAsync(Guid companyId);
        Task<RoleDto?> GetRoleByIdAsync(Guid companyId, Guid id);
        Task<RoleDto?> CreateRoleAsync(Guid companyId, RoleDto roleDto);
        Task<bool> UpdateRoleAsync(Guid companyId, Guid id, RoleDto roleDto);
        Task<bool> DeleteRoleAsync(Guid companyId, Guid id);
    }
}