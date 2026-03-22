using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Enums;

namespace MiniInventorySales.Infrastructure.Persistence
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await db.Database.MigrateAsync();

            if (await db.Users.AnyAsync())
                return;

            var hasher = new PasswordHasher<AppUser>();

            var adminUser = new AppUser
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@user.com",
                Role = UserRole.Admin,
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            var staffUser = new AppUser
            {
                FirstName = "Staff",
                LastName = "User",
                Email = "staff@user.com",
                Role = UserRole.Staff
            };
            staffUser.PasswordHash = hasher.HashPassword(staffUser, "Staff@123");

            await db.Users.AddRangeAsync(adminUser, staffUser);
            await db.SaveChangesAsync();
        }
    }
}