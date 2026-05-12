using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Entities.Translation;
using SimpleECommerceBackend.Infrastructure.Extensions;

namespace SimpleECommerceBackend.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Business entities
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<CustomerShippingAddress> CustomerShippingAddresses => Set<CustomerShippingAddress>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductVariantImage> ProductVariantImages => Set<ProductVariantImage>();
    public DbSet<ProductVariantPrice> ProductVariantPrices => Set<ProductVariantPrice>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewResponse> ReviewResponses => Set<ReviewResponse>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<SellerShop> SellerShops => Set<SellerShop>();
    public DbSet<SellerWarehouse> SellerWarehouses => Set<SellerWarehouse>();

    // Translation
    public DbSet<TranslationEntry> TranslationEntries => Set<TranslationEntry>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw ex.ToConflictException(this);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DbSchemas.Business);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ApplyCommonConventions(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ApplyCommonConventions(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // ===== IEntity =====
            if (typeof(IEntity).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .HasKey(nameof(IEntity.Id));

                modelBuilder.Entity(clrType)
                    .Property(nameof(IEntity.Id))
                    .ValueGeneratedNever();
            }

            // ===== ICreatedTime =====
            if (typeof(ICreatedTrackable).IsAssignableFrom(clrType))
                modelBuilder.Entity(clrType)
                    .Property(nameof(ICreatedTrackable.CreatedAt))
                    .IsRequired();

            // ===== IUpdatedTime =====
            if (typeof(IUpdatedTrackable).IsAssignableFrom(clrType))
                modelBuilder.Entity(clrType)
                    .Property(nameof(IUpdatedTrackable.UpdatedAt));

            // ===== ISoftDeletable =====
            if (typeof(ISoftDeleteTrackable).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .Property(nameof(ISoftDeleteTrackable.IsDeleted))
                    .HasDefaultValue(false);

                modelBuilder.Entity(clrType)
                    .Property(nameof(ISoftDeleteTrackable.DeletedAt));
            }
        }
    }
}
