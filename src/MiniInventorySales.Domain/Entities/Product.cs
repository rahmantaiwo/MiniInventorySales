namespace MiniInventorySales.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }

        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}

