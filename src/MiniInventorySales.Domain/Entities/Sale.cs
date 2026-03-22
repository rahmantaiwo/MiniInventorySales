using MiniInventorySales.Domain.Enums;

namespace MiniInventorySales.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public Guid SoldByUserId { get; set; }
        public AppUser? SoldByUser { get; set; }

        public decimal TotalAmount { get; set; }
        public DateTime SoldAt { get; set; } = DateTime.UtcNow;

        public SaleStatus Status { get; set; } = SaleStatus.Pending;

        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    }
}
