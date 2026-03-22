using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniInventorySales.Domain.Entities;

namespace MiniInventorySales.Infrastructure.Persistence.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Sku).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Sku).IsUnique();

            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Sku).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Sku).IsUnique();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.HasIndex(x => x.Name);

            builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();

            builder.HasQueryFilter(x => x.IsActive);

            builder.Property(x => x.QuantityInStock).IsRequired();
            builder.Property(x => x.ReorderLevel).IsRequired();

            builder.Property(x => x.QuantityInStock).IsRequired();
            builder.Property(x => x.ReorderLevel).IsRequired();
        }
    }
}
