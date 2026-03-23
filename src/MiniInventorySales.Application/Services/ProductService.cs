using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Products;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Domain.Entities;
using MiniInventorySales.Domain.Interface;

namespace MiniInventorySales.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _products;
        private readonly IImageStorage _images;
        private readonly IValidator<CreateProductRequest> _createValidator;
        private readonly IValidator<UpdateProductRequest> _updateValidator;

        public ProductService(
        IProductRepository productRepository,
        IImageStorage imageStorage,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator)
        {
            _products = productRepository;
            _images = imageStorage;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<BaseResponse<Guid>> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(request, ct);
                if (!validationResult.IsValid)
                {
                    return BaseResponse<Guid>.Failure(
                        "Validation failed.",
                        validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                }

                var existingProduct = await _products.GetBySkuAsync(request.Sku.Trim(), ct);
                if (existingProduct is not null)
                    return BaseResponse<Guid>.Failure("A product with this SKU already exists.");

                string? imageUrl = null;
                if (request.Image is not null && request.Image.Content.Length > 0)
                {
                    imageUrl = await _images.UploadAsync(request.Image, ct);
                }

                var product = new Product
                {
                    Sku = request.Sku.Trim(),
                    Name = request.Name.Trim(),
                    UnitPrice = request.UnitPrice,
                    QuantityInStock = request.QuantityInStock,
                    ReorderLevel = request.ReorderLevel,
                    ImageUrl = imageUrl,
                    IsActive = true
                };

                await _products.AddAsync(product, ct);

                return BaseResponse<Guid>.IsSuccessful(product.Id, "Product created successfully.");
            }
            catch (Exception ex)
            {
                return BaseResponse<Guid>.Failure("Failed to create product.", ex.Message);
            }
        }

        public async Task<BaseResponse<string>> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var productItem = await _products.DeactivateProduct(id, ct);
                if (!productItem)
                    return BaseResponse<string>.Failure("Deactivate failed.", "Product not found or already inactive.");

                return BaseResponse<string>.IsSuccessful("OK", "Product deactivated successfully.");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.Failure("Failed to deactivate product.", ex.Message);
            }
        }

        public async Task<BaseResponse<PagedResult<ProductListDto>>> GetAllAsync(ProductQueryRequest request, CancellationToken ct = default)
        {
            try
            {
                var query = _products.Query();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLower();
                    query = query.Where(x =>
                        x.Name.ToLower().Contains(search) ||
                        x.Sku.ToLower().Contains(search));
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == request.IsActive.Value);
                }

                var totalCount = await query.CountAsync(ct);

                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new ProductListDto
                    {
                        Id = x.Id,
                        Sku = x.Sku,
                        Name = x.Name,
                        UnitPrice = x.UnitPrice,
                        QuantityInStock = x.QuantityInStock,
                        ReorderLevel = x.ReorderLevel,
                        ImageUrl = x.ImageUrl,
                        IsActive = x.IsActive
                    })
                    .ToListAsync(ct);

                var pagedResult = new PagedResult<ProductListDto>
                {
                    Items = items,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };

                return BaseResponse<PagedResult<ProductListDto>>.IsSuccessful(pagedResult, "Products retrieved successfully.");
            }
            catch (Exception ex)
            {
                return BaseResponse<PagedResult<ProductListDto>>.Failure("Failed to retrieve products.", ex.Message);
            }
        }

        public async Task<BaseResponse<ProductDetailsDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var product = await _products.GetByIdAsync(id, ct);
                if (product is null)
                    return BaseResponse<ProductDetailsDto>.Failure("Not found.", "Product does not exist.");

                var dto = new ProductDetailsDto
                {
                    Id = product.Id,
                    Sku = product.Sku,
                    Name = product.Name,
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock,
                    ReorderLevel = product.ReorderLevel,
                    ImageUrl = product.ImageUrl,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt
                };

                return BaseResponse<ProductDetailsDto>.IsSuccessful(dto, "Product retrieved successfully.");
            }
            catch (Exception ex)
            {
                return BaseResponse<ProductDetailsDto>.Failure("Failed to retrieve product.", ex.Message);
            }
        }

        public async Task<BaseResponse<ProductDetailsDto>> UpdateAsync(UpdateProductRequest request, CancellationToken ct = default)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(request, ct);
                if (!validationResult.IsValid)
                {
                    return BaseResponse<ProductDetailsDto>.Failure(
                        "Validation failed.",
                        validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                }

                var product = await _products.GetByIdAsync(request.Id, ct);
                if (product is null)
                    return BaseResponse<ProductDetailsDto>.Failure("Product not found.");

                var existingSku = await _products.GetBySkuAsync(request.Sku.Trim(), ct);
                if (existingSku is not null && existingSku.Id != request.Id)
                    return BaseResponse<ProductDetailsDto>.Failure("Another product already uses this SKU.");

                if (request.NewImage is not null && request.NewImage.Content.Length > 0)
                {
                    product.ImageUrl = await _images.UploadAsync(request.NewImage, ct);
                }

                product.Sku = request.Sku.Trim();
                product.Name = request.Name.Trim();
                product.UnitPrice = request.UnitPrice;
                product.QuantityInStock = request.QuantityInStock;
                product.ReorderLevel = request.ReorderLevel;
                product.IsActive = request.IsActive;

                await _products.UpdateAsync(product, ct);

                var dto = new ProductDetailsDto
                {
                    Id = product.Id,
                    Sku = product.Sku,
                    Name = product.Name,
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock,
                    ReorderLevel = product.ReorderLevel,
                    ImageUrl = product.ImageUrl,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt
                };

                return BaseResponse<ProductDetailsDto>.IsSuccessful(dto, "Product updated successfully.");
            }
            catch (Exception ex)
            {
                return BaseResponse<ProductDetailsDto>.Failure("Failed to update product.", ex.Message);
            }
        }
    }
}
