using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories", "dbo");

        builder.Property(i => i.QuantityInStock)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.QuantityReserved)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.Version)
            .IsRequired();

        builder.HasOne(i => i.Product)
            .WithMany()
            .IsRequired()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.SellerWarehouse)
            .WithMany()
            .IsRequired()
            .HasForeignKey(i => i.SellerWarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.ProductId, i.SellerWarehouseId })
            .IsUnique();
    }
}