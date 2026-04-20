using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniInventorySales.Application.Interface;
using MiniInventorySales.Application.Interface.Services;
using MiniInventorySales.Domain.Interface;
using MiniInventorySales.Infrastructure.Persistence;
using MiniInventorySales.Infrastructure.Repositories;
using MiniInventorySales.Infrastructure.Services;
using MiniInventorySales.Infrastructure.Storage;
using MiniInventorySales.Infrastructure.Settings;

namespace MiniInventorySales.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpSection = configuration.GetSection("SmtpSettings");
            if (!smtpSection.Exists())
                smtpSection = configuration.GetSection("Logging:SmtpSettings");

            services.Configure<SmtpSettings>(smtpSection);

            services.AddDbContext<AppDbContext>(options =>
            {
                var cs = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(cs);
            });


            // Repository DI
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped< ISaleRepository, SaleRepository> ();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IImageStorage, CloudinaryImageStorage>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            return services;
        }
    }
}
