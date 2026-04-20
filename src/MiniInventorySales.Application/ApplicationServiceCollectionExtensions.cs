using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MiniInventorySales.Application.DTOs.Products;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Application.Services;
using MiniInventorySales.Application.Validation.Products;

namespace MiniInventorySales.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Use-case services (Application layer)
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddValidatorsFromAssembly(typeof(CreateProductRequestValidator).Assembly);
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
            services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();

            return services;
        }
    }
}
