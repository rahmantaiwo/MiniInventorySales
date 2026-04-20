using FluentValidation;
using MiniInventorySales.Application.DTOs.Products;

namespace MiniInventorySales.Application.Validation.Products
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required.");
            RuleFor(x => x.CostPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Cost price cannot be negative.");
            RuleFor(x => x.SellingPrice)
                .GreaterThan(0).WithMessage("Selling price must be greater than zero.");
            RuleFor(x => x.QuantityInStock)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity in stock cannot be negative.");
            RuleFor(x => x.ReorderLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Reorder level cannot be negative.");

            // Optional: basic image validation if Image is provided
            When(x => x.Image != null, () =>
            {
                RuleFor(x => x.Image!.Content)
                    .NotNull()
                    .Must(c => c.Length > 0).WithMessage("Image file is empty.")
                    .Must(c => c.Length <= 2 * 1024 * 1024).WithMessage("Image must be <= 2MB.");
            });
        }
    }
}
