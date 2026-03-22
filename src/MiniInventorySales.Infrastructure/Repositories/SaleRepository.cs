using Microsoft.EntityFrameworkCore;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Enums;
using MiniInventorySales.Domain.Interface;
using MiniInventorySales.Infrastructure.Persistence;

namespace MiniInventorySales.Infrastructure.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _db;
        public SaleRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Sale sale, CancellationToken ct = default)
        {
            _db.Sales.AddAsync(sale, ct);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> CancelSale(Guid saleId, CancellationToken ct = default)
        {
            var affectedRows = await _db.Sales
                .Where(s => s.Id == saleId)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.Status, SaleStatus.Cancelled), ct);

            return affectedRows > 0;
        }

        public IQueryable<Sale> Query()
        {
            return
            _db.Sales.Include(x => x.Items)
                .AsNoTracking()
                .AsQueryable();
        }
    }
}
