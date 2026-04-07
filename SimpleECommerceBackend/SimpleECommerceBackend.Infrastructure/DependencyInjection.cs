using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Translation;
using SimpleECommerceBackend.Application.Interfaces.Services.Address;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Infrastructure.Persistence;
using SimpleECommerceBackend.Infrastructure.Persistence.Interceptors;
using SimpleECommerceBackend.Infrastructure.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Repositories.Translation;
using SimpleECommerceBackend.Infrastructure.Services.Address;
using SimpleECommerceBackend.Infrastructure.Services.Email;
using SimpleECommerceBackend.Infrastructure.Services.Translation;

namespace SimpleECommerceBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Email Services
        services.Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"));
        services.AddScoped<IEmailBodyProvider, EmailBodyProvider>();
        services.AddSingleton<BackgroundEmailQueue>();
        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddHostedService<EmailBackgroundWorker>();

        // Address Services
        services.AddSingleton<IAddressService, VnAddressService>();

        // Translation Services
        services.Configure<TranslationOptions>(configuration.GetSection(TranslationOptions.SectionName));
        services.Configure<OpenAiTranslationOptions>(configuration.GetSection(OpenAiTranslationOptions.SectionName));
        services.Configure<GoogleAiTranslationOptions>(
            configuration.GetSection(GoogleAiTranslationOptions.SectionName));

        var redisConnectionString = configuration["Translation:Redis:ConnectionString"];
        var redisInstanceName = configuration["Translation:Redis:InstanceName"];
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = redisInstanceName;
            });
        else
            services.AddDistributedMemoryCache();

        services.AddSingleton<IStaticTextLocalizer, JsonStaticTextLocalizer>();
        services.AddScoped<IDynamicTranslationService, DynamicTranslationService>();
        services.AddScoped<ITranslationCache, DistributedTranslationCache>();
        services.AddScoped<ITranslationEntryRepository, TranslationEntryRepository>();

        services.AddSingleton<NullTranslationProvider>();
        services.AddSingleton<ITranslationProvider>(sp => sp.GetRequiredService<NullTranslationProvider>());
        services.AddHttpClient<OpenAiTranslationProvider>();
        services.AddHttpClient<GoogleAiTranslationProvider>();
        services.AddScoped<ITranslationProvider>(sp => sp.GetRequiredService<OpenAiTranslationProvider>());
        services.AddScoped<ITranslationProvider>(sp => sp.GetRequiredService<GoogleAiTranslationProvider>());

        // Database
        services.AddScoped<AuditSaveChangesInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Business Repositories
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerShippingAddressRepository, CustomerShippingAddressRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISellerShopRepository, SellerShopRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        return services;
    }
}