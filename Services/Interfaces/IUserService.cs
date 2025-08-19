using InvoicingSystem.DTOs;

namespace InvoicingSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetUsersAsync(Guid companyId);
        Task<UserReadDto?> GetUserByIdAsync(Guid companyId, Guid id);
        Task<UserReadDto?> CreateUserAsync(Guid companyId, UserCreateDto dto);
        Task<(bool Success, bool Conflict)> UpdateUserAsync(Guid companyId, Guid id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(Guid companyId, Guid id);
    }
}