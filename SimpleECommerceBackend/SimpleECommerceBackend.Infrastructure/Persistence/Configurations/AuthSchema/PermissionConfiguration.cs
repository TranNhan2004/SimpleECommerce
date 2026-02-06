using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Auth;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.AuthSchema;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", "auth");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(PermissionConstants.NameMaxLength);

        builder.Property(r => r.Description)
            .HasMaxLength(PermissionConstants.DescriptionMaxLength)
            .HasDefaultValue(null);

        builder.HasIndex(c => c.Name).IsUnique();
    }
}