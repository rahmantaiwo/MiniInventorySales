using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Categories;

namespace MiniInventorySales.Application.Interface
{
    public interface ICategoryService
    {
        Task<BaseResponse<List<CategoryListDto>>> GetAllAsync(CancellationToken ct = default);
    }
}
