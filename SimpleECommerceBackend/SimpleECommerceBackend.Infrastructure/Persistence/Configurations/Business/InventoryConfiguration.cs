using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories", DbSchemas.Business);

        builder.Property(i => i.QuantityInStock)
            .IsRequired();

        builder.Property(i => i.QuantityReserved)
            .IsRequired();

        builder.Property(i => i.Version)
            .IsRequired();

        builder.HasOne(i => i.ProductVariant)
            .WithMany()
            .IsRequired()
            .HasForeignKey(i => i.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.SellerWarehouse)
            .WithMany()
            .IsRequired()
            .HasForeignKey(i => i.SellerWarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.ProductVariantId, i.SellerWarehouseId })
            .IsUnique();
    }
}
