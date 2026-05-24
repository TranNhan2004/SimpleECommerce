using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Uam;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Uam;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", DbSchemas.Uam);

        builder.Property(permission => permission.Code)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(permission => permission.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(permission => permission.Description)
            .HasMaxLength(512);

        builder.HasIndex(permission => permission.Code)
            .IsUnique();
    }
}
