using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages", "dbo");

        builder.Property(pi => pi.ImageUrl)
            .IsRequired();

        builder.Property(pi => pi.DisplayOrder)
            .IsRequired();

        builder.Property(pi => pi.IsDisplayed)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pi => pi.Description)
            .HasMaxLength(ProductImageConstants.DescriptionMaxLength);

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages)
            .IsRequired()
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pi => pi.ProductId);
    }
}