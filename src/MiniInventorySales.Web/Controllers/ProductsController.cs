using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniInventorySales.Application.Common;
using MiniInventorySales.Application.DTOs.Image;
using MiniInventorySales.Application.DTOs.Products;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Web.Models.Products;

namespace MiniInventorySales.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _products;

        public ProductsController(IProductService products) => _products = products;

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ProductQueryRequest query, CancellationToken ct)
        {
            NormalizeQuery(query);

            var result = await _products.GetAllAsync(query, ct);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Errors.FirstOrDefault() ?? result.Message;
                return View(new ProductIndexViewModel { Query = query, Paged = new PagedResult<ProductListDto>() });
            }

            var vm = new ProductIndexViewModel
            {
                Query = query,
                Paged = result.Data
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View(new ProductCreateViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var request = new CreateProductRequest
            {
                Sku = model.Sku,
                Name = model.Name,
                UnitPrice = model.UnitPrice,
                QuantityInStock = model.QuantityInStock,
                ReorderLevel = model.ReorderLevel,
                Image = await MapImageAsync(model.Image, ct)
            };

            var result = await _products.CreateAsync(request, ct);
            if (!result.Success)
            {
                AddErrorsToModelState(result);
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
                UnitPrice = result.Data.UnitPrice,
                QuantityInStock = result.Data.QuantityInStock,
                ReorderLevel = result.Data.ReorderLevel,
                IsActive = result.Data.IsActive,
                ExistingImageUrl = result.Data.ImageUrl
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var request = new UpdateProductRequest
            {
                Id = model.Id,
                Sku = model.Sku,
                Name = model.Name,
                UnitPrice = model.UnitPrice,
                QuantityInStock = model.QuantityInStock,
                ReorderLevel = model.ReorderLevel,
                IsActive = model.IsActive,
                NewImage = await MapImageAsync(model.NewImage, ct)
            };

            var result = await _products.UpdateAsync(request, ct);
            if (!result.Success)
            {
                AddErrorsToModelState(result);
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
