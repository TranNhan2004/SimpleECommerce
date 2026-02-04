using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Entities.Auth;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.AuthSchema;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "auth");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(RoleConstants.NameMaxLength);

        builder.HasIndex(c => c.Name).IsUnique();
    }
}