using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles", "dbo");
        builder.Property(u => u.CredentialId);
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(CredentialConstants.EmailMaxLength);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(UserProfileConstants.FirstNameMaxLength);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(UserProfileConstants.LastNameMaxLength);

        builder.Property(u => u.BirthDate).HasDefaultValue(null);

        builder.HasIndex(u => u.CredentialId).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasOne(c => c.Credential)
            .WithMany()
            .IsRequired()
            .HasForeignKey(c => c.CredentialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}