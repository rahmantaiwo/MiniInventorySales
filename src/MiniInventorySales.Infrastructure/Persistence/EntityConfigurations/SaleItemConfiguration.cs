using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniInventorySales.Domain.Entities;

namespace MiniInventorySales.Infrastructure.Persistence.EntityConfigurations
{
    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("SaleITems");

            builder.HasKey(x => x.Id);

           builder.Property(x => x.UnitPriceAtSale)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(x => x.LineTotal)
            .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Sale)
                .WithMany(s => s.Items)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.SaleId);
            builder.HasIndex(x => x.ProductId);
        }
    }
}
