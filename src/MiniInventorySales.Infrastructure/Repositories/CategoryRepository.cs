using Microsoft.EntityFrameworkCore;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Interface;
using MiniInventorySales.Infrastructure.Persistence;

namespace MiniInventorySales.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _db;

        public CategoryRepository(AppDbContext db) => _db = db;

        public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public Task<List<Category>> GetAllAsync(CancellationToken ct = default)
        {
            return _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Category category, CancellationToken ct = default)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync(ct);
        }
    }
}
