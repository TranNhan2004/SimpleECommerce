using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Entities.AuditTracking;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Entities.Translation;
using SimpleECommerceBackend.Domain.Entities.Uam;
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
    public DbSet<SellerShop> SellerShops => Set<SellerShop>();
    public DbSet<SellerWarehouse> SellerWarehouses => Set<SellerWarehouse>();

    // UAM
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Translation
    public DbSet<TranslationEntry> TranslationEntries => Set<TranslationEntry>();

    // Audit Tracking
    public DbSet<Audit> Audits => Set<Audit>();

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

            // ===== EntityBase =====
            if (typeof(EntityBase).IsAssignableFrom(clrType))
            {
                var entity = modelBuilder.Entity(clrType);

                entity.Property(nameof(EntityBase.CreatedAt))
                    .IsRequired();

                entity.Property(nameof(EntityBase.CreatedById))
                    .IsRequired();

                entity.Property(nameof(EntityBase.UpdatedAt));
                entity.Property(nameof(EntityBase.UpdatedById));
                entity.Property(nameof(EntityBase.IsDeleted));
                entity.Property(nameof(EntityBase.DeletedAt));
                entity.Property(nameof(EntityBase.DeletedById));

                entity.HasOne(nameof(EntityBase.CreatedBy))
                    .WithMany()
                    .HasForeignKey(nameof(EntityBase.CreatedById))
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(nameof(EntityBase.UpdatedBy))
                    .WithMany()
                    .HasForeignKey(nameof(EntityBase.UpdatedById))
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(nameof(EntityBase.DeletedBy))
                    .WithMany()
                    .HasForeignKey(nameof(EntityBase.DeletedById))
                    .OnDelete(DeleteBehavior.Restrict);

                // Is Deleted Query Filter 
                var parameter = Expression.Parameter(clrType, "e");

                var isDeletedProperty = Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    [typeof(bool)],
                    parameter,
                    Expression.Constant(nameof(EntityBase.IsDeleted))
                );

                var filter = Expression.Lambda(
                    Expression.Not(isDeletedProperty),
                    parameter
                );

                entity.HasQueryFilter(filter);
            }
        }
    }
}
