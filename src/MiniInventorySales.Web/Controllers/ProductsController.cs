using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Image;
using MiniInventorySales.Application.DTOs.Products;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Web.Models.Products;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MiniInventorySales.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _products;
        private readonly ICategoryService _categories;

        public ProductsController(IProductService products, ICategoryService categories)
        {
            _products = products;
            _categories = categories;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ProductQueryRequest query, CancellationToken ct)
        {
            NormalizeQuery(query);

            var result = await _products.GetAllAsync(query, ct);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Errors.FirstOrDefault() ?? result.Message;
                var emptyVm = new ProductIndexViewModel
                {
                    Query = query,
                    Paged = new PagedResult<ProductListDto>(),
                    CategoryOptions = await BuildCategoryOptionsAsync(query.CategoryId, ct)
                };
                return View(emptyVm);
            }

            var vm = new ProductIndexViewModel
            {
                Query = query,
                Paged = result.Data,
                CategoryOptions = await BuildCategoryOptionsAsync(query.CategoryId, ct)
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var vm = new ProductCreateViewModel
            {
                CategoryOptions = await BuildCategoryOptionsAsync(null, ct)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model, CancellationToken ct)
        {
            if (model.CategoryId == Guid.Empty)
                ModelState.AddModelError(nameof(model.CategoryId), "Category is required.");

            if (!ModelState.IsValid)
            {
                model.CategoryOptions = await BuildCategoryOptionsAsync(model.CategoryId, ct);
                return View(model);
            }

            var request = new CreateProductRequest
            {
                Sku = model.Sku,
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                CostPrice = model.CostPrice,
                SellingPrice = model.SellingPrice,
                QuantityInStock = model.QuantityInStock,
                ReorderLevel = model.ReorderLevel,
                Image = await MapImageAsync(model.Image, ct)
            };

            var result = await _products.CreateAsync(request, ct);
            if (!result.Success)
            {
                AddErrorsToModelState(result);
                model.CategoryOptions = await BuildCategoryOptionsAsync(model.CategoryId, ct);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var result = await _products.GetByIdAsync(id, ct);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Errors.FirstOrDefault() ?? result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var result = await _products.GetByIdAsync(id, ct);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Errors.FirstOrDefault() ?? result.Message;
                return RedirectToAction(nameof(Index));
            }

            var vm = new ProductEditViewModel
            {
                Id = result.Data.Id,
                Sku = result.Data.Sku,
                Name = result.Data.Name,
                Description = result.Data.Description,
                CategoryId = result.Data.CategoryId,
                CostPrice = result.Data.CostPrice,
                SellingPrice = result.Data.SellingPrice,
                QuantityInStock = result.Data.QuantityInStock,
                ReorderLevel = result.Data.ReorderLevel,
                IsActive = result.Data.IsActive,
                ExistingImageUrl = result.Data.ImageUrl,
                CategoryOptions = await BuildCategoryOptionsAsync(result.Data.CategoryId, ct)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel model, CancellationToken ct)
        {
            if (model.CategoryId == Guid.Empty)
                ModelState.AddModelError(nameof(model.CategoryId), "Category is required.");

            if (!ModelState.IsValid)
            {
                model.CategoryOptions = await BuildCategoryOptionsAsync(model.CategoryId, ct);
                return View(model);
            }

            var request = new UpdateProductRequest
            {
                Id = model.Id,
                Sku = model.Sku,
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                CostPrice = model.CostPrice,
                SellingPrice = model.SellingPrice,
                QuantityInStock = model.QuantityInStock,
                ReorderLevel = model.ReorderLevel,
                IsActive = model.IsActive,
                NewImage = await MapImageAsync(model.NewImage, ct)
            };

            var result = await _products.UpdateAsync(request, ct);
            if (!result.Success)
            {
                AddErrorsToModelState(result);
                model.CategoryOptions = await BuildCategoryOptionsAsync(model.CategoryId, ct);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            var result = await _products.DeactivateAsync(id, ct);
            if (!result.Success)
            {
                TempData["Error"] = result.Errors.FirstOrDefault() ?? result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        private static void NormalizeQuery(ProductQueryRequest query)
        {
            if (query.PageNumber < 1) query.PageNumber = 1;
            if (query.PageSize < 1) query.PageSize = 10;
            if (query.PageSize > 50) query.PageSize = 50;
        }

        private async Task<List<SelectListItem>> BuildCategoryOptionsAsync(Guid? selectedId, CancellationToken ct)
        {
            var result = await _categories.GetAllAsync(ct);
            var items = result.Data ?? new List<Application.DTOs.Categories.CategoryListDto>();

            var options = items.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = selectedId.HasValue && c.Id == selectedId.Value
            }).ToList();

            options.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- Select Category --",
                Selected = !selectedId.HasValue || selectedId == Guid.Empty
            });

            return options;
        }

        private void AddErrorsToModelState<T>(BaseResponse<T> result)
        {
            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error))
                        ModelState.AddModelError(string.Empty, error);
                }
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }
        }

        private static async Task<ImageUploadRequest?> MapImageAsync(IFormFile? file, CancellationToken ct)
        {
            if (file is null || file.Length == 0) return null;

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);

            return new ImageUploadRequest
            {
                Content = ms.ToArray(),
                FileName = file.FileName,
                ContentType = file.ContentType
            };
        }
    }
}
