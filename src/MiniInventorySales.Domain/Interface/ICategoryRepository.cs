using MiniInventorySales.Domain.Entities;

namespace MiniInventorySales.Domain.Interface
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Category>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Category category, CancellationToken ct = default);
    }
}
