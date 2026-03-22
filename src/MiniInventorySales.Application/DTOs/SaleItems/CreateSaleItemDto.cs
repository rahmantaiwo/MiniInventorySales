using System.ComponentModel.DataAnnotations;

namespace MiniInventorySales.Application.DTOs.SaleItems
{
    public class CreateSaleItemDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, 1_000_000)]
        public int Quantity { get; set; }
    }
}
