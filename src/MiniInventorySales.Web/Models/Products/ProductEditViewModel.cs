using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal CostPrice { get; set; }

        [Range(0.01, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal SellingPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ExistingImageUrl { get; set; }

        public IFormFile? NewImage { get; set; }

        public List<SelectListItem> CategoryOptions { get; set; } = new();
    }
}
