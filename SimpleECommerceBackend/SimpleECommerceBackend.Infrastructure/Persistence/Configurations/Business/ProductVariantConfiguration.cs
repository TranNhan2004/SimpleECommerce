using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants", DbSchemas.Business);

        builder.Property(pv => pv.Name)
            .IsRequired()
            .HasMaxLength(ProductVariantValidationRules.NameMaxLength);

        builder.Property(pv => pv.Description)
            .IsRequired()
            .HasMaxLength(ProductVariantValidationRules.DescriptionMaxLength);

        builder.ComplexProperty(pv => pv.CurrentPrice, money =>
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

        builder.Property(pv => pv.TotalInStock)
            .IsRequired();

        builder.Property(pv => pv.DefaultImageUrl);

        builder.Property(pv => pv.Status)
            .IsRequired();

        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.ProductVariants)
            .IsRequired()
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pv => pv.ProductId);
    }
}
