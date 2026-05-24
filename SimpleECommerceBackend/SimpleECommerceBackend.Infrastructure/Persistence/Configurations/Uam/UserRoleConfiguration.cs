using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Uam;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Uam;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles", DbSchemas.Uam);

        builder.HasOne(userRole => userRole.User)
            .WithMany()
            .IsRequired()
            .HasForeignKey(userRole => userRole.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(userRole => userRole.Role)
            .WithMany()
            .IsRequired()
            .HasForeignKey(userRole => userRole.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(userRole => new { userRole.UserId, userRole.RoleId })
            .IsUnique();
    }
}
