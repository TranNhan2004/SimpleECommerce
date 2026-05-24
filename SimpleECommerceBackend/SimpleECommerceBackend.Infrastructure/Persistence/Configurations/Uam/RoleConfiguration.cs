using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Uam;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Uam;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", DbSchemas.Uam);

        builder.Property(role => role.Code)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(role => role.Description)
            .HasMaxLength(512);

        builder.HasIndex(role => role.Code)
            .IsUnique();
    }
}
