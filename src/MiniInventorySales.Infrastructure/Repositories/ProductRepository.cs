using Microsoft.EntityFrameworkCore;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Interface;
using MiniInventorySales.Infrastructure.Persistence;

namespace MiniInventorySales.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Product product, CancellationToken ct = default)
        {
            _db.Products.AddAsync(product, ct);
            await _db.SaveChangesAsync(ct);

        }

        public async Task<bool> DeactivateProduct(Guid id, CancellationToken ct = default)
        {
            var affectedRows =await _db.Products
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.IsActive, false), ct);

            return affectedRows > 0;
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return
            await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default)
        {
            return
            _db.Products.FirstOrDefaultAsync(p => p.Sku == sku, ct);
        }

        public IQueryable<Product> Query()
        {
            return _db.Products
                .AsNoTracking()
                .AsQueryable();
        }

        public Task UpdateAsync(Product product, CancellationToken ct = default)
        {
            _db.Products.Update(product);
            return _db.SaveChangesAsync(ct);
        }
    }
}
