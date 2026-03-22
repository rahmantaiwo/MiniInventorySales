namespace MiniInventorySales.Application.DTOs.Products
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
