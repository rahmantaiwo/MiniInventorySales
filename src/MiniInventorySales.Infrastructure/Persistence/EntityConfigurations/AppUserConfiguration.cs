using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniInventorySales.Domain.Entities;

namespace MiniInventorySales.Infrastructure.Persistence.EntityConfigurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AppUsers");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.IsActive).IsRequired();
            builder.Property(u => u.Role).IsRequired();

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();

            builder.HasQueryFilter(x => x.IsActive);

            builder.Property(u => u.CreatedAt).IsRequired();
        }
    }
}
