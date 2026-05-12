using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", DbSchemas.Business);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(ProductValidationRules.NameMaxLength);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(ProductValidationRules.DescriptionMaxLength);

        builder.Property(p => p.AverageRating)
            .HasColumnType("decimal(3,2)")
            .IsRequired();

        builder.Property(p => p.TotalRatings)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasDefaultValue(ProductStatus.Active)
            .IsRequired();

        builder.HasOne(p => p.Category)
            .WithMany()
            .IsRequired()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Seller)
            .WithMany()
            .IsRequired()
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.SellerId);
    }
}
