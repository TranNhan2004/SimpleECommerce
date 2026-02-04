using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", "dbo");
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(CategoryConstants.NameMaxLength);

        builder.Property(c => c.Description)
            .HasDefaultValue(null)
            .HasMaxLength(CategoryConstants.DescriptionMaxLength);
    }
}