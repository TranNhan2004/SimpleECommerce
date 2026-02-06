using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Auth;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.AuthSchema;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions", "auth");

        builder.HasOne(r => r.Role)
            .WithMany()
            .IsRequired()
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Permission)
            .WithMany()
            .IsRequired()
            .HasForeignKey(r => r.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}