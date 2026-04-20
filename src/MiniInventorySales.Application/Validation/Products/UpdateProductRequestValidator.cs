using FluentValidation;
using MiniInventorySales.Application.DTOs.Products;

namespace MiniInventorySales.Application.Validation.Products
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);

            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SellingPrice).GreaterThan(0);
            RuleFor(x => x.QuantityInStock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);

            When(x => x.NewImage != null, () =>
            {
                RuleFor(x => x.NewImage!.Content)
                    .Must(c => c.Length > 0).WithMessage("Image file is empty.")
                    .Must(c => c.Length <= 2 * 1024 * 1024).WithMessage("Image must be <= 2MB.");
            });
        }
    }
}
