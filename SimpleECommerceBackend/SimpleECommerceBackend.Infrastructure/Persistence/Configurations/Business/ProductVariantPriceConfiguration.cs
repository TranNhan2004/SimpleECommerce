using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class ProductVariantPriceConfiguration : IEntityTypeConfiguration<ProductVariantPrice>
{
    public void Configure(EntityTypeBuilder<ProductVariantPrice> builder)
    {
        builder.ToTable("ProductVariantPrices", DbSchemas.Business);

        builder.Property(pvp => pvp.EffectiveFrom)
            .IsRequired();

        builder.ComplexProperty(pvp => pvp.Money, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasOne(pvp => pvp.ProductVariant)
            .WithMany(pv => pv.ProductVariantPrices)
            .IsRequired()
            .HasForeignKey(pvp => pvp.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pvp => pvp.ProductVariantId);
    }
}
