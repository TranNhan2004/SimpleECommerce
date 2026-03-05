using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Address;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;
using SimpleECommerceBackend.Infrastructure.Persistence;
using SimpleECommerceBackend.Infrastructure.Persistence.Interceptors;
using SimpleECommerceBackend.Infrastructure.Repositories.Auth;
using SimpleECommerceBackend.Infrastructure.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Security;
using SimpleECommerceBackend.Infrastructure.Services.Keycloak;
using SimpleECommerceBackend.Infrastructure.Services.Address;
using SimpleECommerceBackend.Infrastructure.Services.Email;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;

namespace SimpleECommerceBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Email
        services.Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"));
        services.AddScoped<IEmailProvider, EmailProvider>();
        services.AddSingleton<BackgroundEmailQueue>();
        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddHostedService<EmailBackgroundWorker>();

        // Hasher
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // Auth Services
        services.AddSingleton<IJwtGenerator, JwtGenerator>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Keycloak Services
        services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak"));

        services.AddHttpClient<IKeycloakTokenService, KeycloakTokenService>((sp, client) =>
        {
            var keycloakSettings = configuration.GetSection("Keycloak").Get<KeycloakSettings>();
            client.BaseAddress = new Uri(keycloakSettings!.AuthServerUrl);
            client.Timeout = TimeSpan.FromSeconds(keycloakSettings!.TimeoutSeconds);
        });

        services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>((sp, client) =>
        {
            var keycloakSettings = configuration.GetSection("Keycloak").Get<KeycloakSettings>();
            client.BaseAddress = new Uri(keycloakSettings!.AuthServerUrl);
            client.Timeout = TimeSpan.FromSeconds(keycloakSettings!.TimeoutSeconds);
        });

        // Address
        services.AddSingleton<IAddressService, VnAddressService>();

        // Db
        services.AddScoped<AuditSaveChangesInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        // Unit of work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Repositories
        // Auth
        services.AddScoped<ICredentialRepository, CredentialRepository>();

        // Business
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerShippingAddressRepository, CustomerShippingAddressRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISellerShopRepository, SellerShopRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        return services;
    }
}