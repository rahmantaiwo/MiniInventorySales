using Microsoft.EntityFrameworkCore;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Interface;
using MiniInventorySales.Infrastructure.Persistence;

namespace MiniInventorySales.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _db;

        public RoleRepository(AppDbContext db) => _db = db;

        public Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _db.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return _db.Roles.FirstOrDefaultAsync(r => r.Name == name, ct);
        }

        public async Task AddAsync(Role role, CancellationToken ct = default)
        {
            _db.Roles.Add(role);
            await _db.SaveChangesAsync(ct);
        }
    }
}
