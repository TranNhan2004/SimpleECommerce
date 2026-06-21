using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Extensions;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", DbSchemas.Business);
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(CategoryValidationRules.NameMaxLength);

        builder.Property(c => c.Status)
            .HasEnumStringConversion(50)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasDefaultValue(null)
            .HasMaxLength(CategoryValidationRules.DescriptionMaxLength);
    }
}
