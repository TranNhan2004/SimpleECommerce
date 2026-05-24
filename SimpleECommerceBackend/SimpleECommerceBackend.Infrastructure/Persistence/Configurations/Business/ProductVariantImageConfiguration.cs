using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class ProductVariantImageConfiguration : IEntityTypeConfiguration<ProductVariantImage>
{
    public void Configure(EntityTypeBuilder<ProductVariantImage> builder)
    {
        builder.ToTable("ProductVariantImages", DbSchemas.Business);

        builder.Property(pvi => pvi.ImageUrl)
            .IsRequired();

        builder.Property(pvi => pvi.DisplayOrder)
            .IsRequired();

        builder.Property(pvi => pvi.IsDisplayed)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pvi => pvi.Description)
            .HasMaxLength(ProductVariantImageValidationRules.DescriptionMaxLength);

        builder.HasOne(pvi => pvi.ProductVariant)
            .WithMany(pv => pv.ProductVariantImages)
            .IsRequired()
            .HasForeignKey(pvi => pvi.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pvi => pvi.ProductVariantId);
    }
}
