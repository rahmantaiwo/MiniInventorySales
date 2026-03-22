using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Products;

namespace MiniInventorySales.Application.Interface
{
    public interface IProductService
    {
        Task<BaseResponse<PagedResult<ProductListDto>>> GetAllAsync(ProductQueryRequest request, CancellationToken ct = default);
        Task<BaseResponse<ProductDetailsDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<BaseResponse<Guid>> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
        Task<BaseResponse<ProductDetailsDto>> UpdateAsync(UpdateProductRequest request, CancellationToken ct = default);
        Task<BaseResponse<string>> DeactivateAsync(Guid id, CancellationToken ct = default);
    }
}
