namespace MiniInventorySales.Application.DTOs.Products
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
