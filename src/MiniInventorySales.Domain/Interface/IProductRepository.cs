using MiniInventorySales.Domain.Entities;

namespace MiniInventorySales.Domain.Interface
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);
        Task AddAsync(Product product, CancellationToken ct = default);
        Task UpdateAsync(Product product, CancellationToken ct = default);
        Task<bool> DeactivateProduct(Guid id, CancellationToken ct = default);

        IQueryable<Product> Query();
    }
}
