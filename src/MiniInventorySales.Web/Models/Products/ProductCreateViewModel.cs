using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MiniInventorySales.Web.Models.Products
{
    public class ProductCreateViewModel
    {
        [Required]
        [StringLength(50)]
        public string Sku { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Range(0.01, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        public IFormFile? Image { get; set; }
    }
}
