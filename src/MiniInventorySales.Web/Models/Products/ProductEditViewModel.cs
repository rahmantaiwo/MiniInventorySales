using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MiniInventorySales.Web.Models.Products
{
    public class ProductEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Sku { get; set; } = "";

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = "";

        [Range(0.01, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ExistingImageUrl { get; set; }

        public IFormFile? NewImage { get; set; }
    }
}
