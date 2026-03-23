using System.Linq;
using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Products;

namespace MiniInventorySales.Web.Models.Products
{
    public class ProductIndexViewModel
    {
        public ProductQueryRequest Query { get; set; } = new();
        public PagedResult<ProductListDto> Paged { get; set; } = new();

        public int PageLowStockCount => Paged.Items.Count(x => x.QuantityInStock <= x.ReorderLevel);
        public int PageActiveCount => Paged.Items.Count(x => x.IsActive);
    }
}
