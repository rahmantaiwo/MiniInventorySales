using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniInventorySales.Domain.Entities;

namespace MiniInventorySales.Infrastructure.Persistence.EntityConfigurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(s => s.SoldAt)
                .IsRequired();

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.HasOne(s => s.SoldByUser)
                .WithMany(s => s.Sales)
                .HasForeignKey(s => s.SoldByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.SoldAt);
            builder.HasIndex(s => s.SoldByUserId);
        }
    }
}
