using MiniInventorySales.Application.DTOs.Image;

namespace MiniInventorySales.Application.DTOs.Products
{
    public class UpdateProductRequest
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; } = true;
        public ImageUploadRequest? NewImage { get; set; }
    }
}
