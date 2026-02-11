using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Infrastructure.Persistence.DbExceptions;

namespace SimpleECommerceBackend.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Credential> Credentials => Set<Credential>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderShippingAddress> OrderShippingAddresses => Set<OrderShippingAddress>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductPrice> ProductPrices => Set<ProductPrice>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<CustomerShippingAddress> UserShippingAddresses => Set<CustomerShippingAddress>();


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
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly()
        );

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
                    .HasDefaultValueSql("NEWSEQUENTIALID()")
                    .ValueGeneratedOnAdd();
            }

            // ===== ICreatedTime =====
            if (typeof(ICreatedTime).IsAssignableFrom(clrType))
                modelBuilder.Entity(clrType)
                    .Property(nameof(ICreatedTime.CreatedAt))
                    .IsRequired();

            // ===== IUpdatedTime =====
            if (typeof(IUpdatedTime).IsAssignableFrom(clrType))
                modelBuilder.Entity(clrType)
                    .Property(nameof(IUpdatedTime.UpdatedAt));

            // ===== ISoftDeletable =====
            if (typeof(ISoftDeletable).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .Property(nameof(ISoftDeletable.IsDeleted))
                    .HasDefaultValue(false);

                modelBuilder.Entity(clrType)
                    .Property(nameof(ISoftDeletable.DeletedAt));
            }
        }
    }
}