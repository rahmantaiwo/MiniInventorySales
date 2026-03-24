using Microsoft.EntityFrameworkCore;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Interface;
using MiniInventorySales.Infrastructure.Persistence;

namespace MiniInventorySales.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db) => _db = db;

        public async Task AddUserAsync(AppUser user, CancellationToken ct = default)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeactivateUser(Guid id, CancellationToken ct = default)
        {
            var affectedRows = await _db.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.IsActive, false), ct);

            return affectedRows > 0;
        }

        public async Task<AppUser?> GetUserByEmailAsync(string email, CancellationToken ct = default)
        {
            return
                 await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);
        }

        public async Task<AppUser?> GetUserByIdAsync(Guid id, CancellationToken ct = default)
        {
            return 
                await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public Task<List<AppUser>> GetAllAsync(CancellationToken ct = default)
        {
            return _db.Users.ToListAsync(ct);
        }

        public async Task UpdateUserAsync(AppUser user, CancellationToken ct = default)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
