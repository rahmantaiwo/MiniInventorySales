using MiniInventorySales.Domain.Interface;
using MiniInventorySales.Infrastructure.Persistence;

namespace MiniInventorySales.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork   
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }
}
