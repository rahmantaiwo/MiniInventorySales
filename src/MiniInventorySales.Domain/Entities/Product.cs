namespace MiniInventorySales.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public AppUser? CreatedByUser { get; set; }

        public Guid? UpdatedByUserId { get; set; }
        public AppUser? UpdatedByUser { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}

