using MiniInventorySales.Domain.Entities;

namespace MiniInventorySales.Domain.Interface
{
    public interface IUserRepository
    {
        Task<AppUser?> GetUserByEmailAsync(string email, CancellationToken ct = default);
        Task<AppUser?> GetUserByUsernameAsync(string username, CancellationToken ct = default);
        Task<AppUser?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<AppUser>> GetAllAsync(CancellationToken ct = default);
        Task AddUserAsync(AppUser user, CancellationToken ct = default);
        Task UpdateUserAsync(AppUser user, CancellationToken ct = default);
        Task<bool> DeactivateUser(Guid Id, CancellationToken ct = default);
    }
}
