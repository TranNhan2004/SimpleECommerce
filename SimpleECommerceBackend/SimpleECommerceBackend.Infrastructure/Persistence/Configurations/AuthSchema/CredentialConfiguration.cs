using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Entities.Auth;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.AuthSchema;

public class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.ToTable("Credentials", "auth");

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(CredentialConstants.EmailMaxLength);

        builder.Property(c => c.PasswordHash).IsRequired();

        builder.HasOne(c => c.Role)
            .WithMany()
            .IsRequired()
            .HasForeignKey(c => c.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}