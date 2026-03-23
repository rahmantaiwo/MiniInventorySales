using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Domain.Interface;
using MiniInventorySales.Infrastructure.Persistence;
using MiniInventorySales.Infrastructure.Repositories;
using MiniInventorySales.Infrastructure.Storage;

namespace MiniInventorySales.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                var cs = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(cs);
            });


            // Repository DI
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped< ISaleRepository, SaleRepository> ();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IImageStorage, CloudinaryImageStorage>();
            return services;
        }
    }
}
