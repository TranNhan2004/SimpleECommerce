using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

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

        builder.Property(c => c.AdminId);

        builder.HasOne(c => c.Admin)
            .WithMany()
            .IsRequired()
            .HasForeignKey(c => c.AdminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}