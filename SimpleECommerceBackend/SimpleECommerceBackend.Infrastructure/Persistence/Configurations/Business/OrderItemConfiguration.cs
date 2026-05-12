using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", DbSchemas.Business);

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.ComplexProperty(oi => oi.CurrentPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("CurrentAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasOne(oi => oi.ProductVariant)
            .WithMany()
            .IsRequired()
            .HasForeignKey(oi => oi.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .IsRequired()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(oi => new { oi.OrderId, oi.ProductVariantId })
            .IsUnique();
    }
}
