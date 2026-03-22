using MiniInventorySales.Application.DTOs.Image;

namespace MiniInventorySales.Application.DTOs.Products
{
    public class CreateProductRequest
    {
        public string Sku { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public ImageUploadRequest? Image { get; set; } // single image
    }
}
