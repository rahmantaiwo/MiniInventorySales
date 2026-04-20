using Microsoft.Extensions.Logging;
using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Categories;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Domain.Interface;

namespace MiniInventorySales.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categories, ILogger<CategoryService> logger)
        {
            _categories = categories;
            _logger = logger;
        }

        public async Task<BaseResponse<List<CategoryListDto>>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var items = await _categories.GetAllAsync(ct);
                var dtos = items.Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                return BaseResponse<List<CategoryListDto>>.IsSuccessful(dtos, "Categories retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve categories");
                return BaseResponse<List<CategoryListDto>>.Failure("Failed to retrieve categories.", ex.Message);
            }
        }
    }
}
