using MiniInventorySales.Application.DTOs.SaleItems;
using System.ComponentModel.DataAnnotations;

namespace MiniInventorySales.Application.DTOs.Sales
{
    public class CreateSaleDto
    {
        [Required]
        public List<CreateSaleItemDto> Items { get; set; } = new();
    }
}
