using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("Users", "dbo");
        builder.Property(u => u.CredentialId);
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(UserConstants.EmailMaxLength);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(UserConstants.PhoneNumberExactLength);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(UserConstants.FirstNameMaxLength);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(UserConstants.LastNameMaxLength);

        builder.Property(u => u.BirthDate).HasDefaultValue(null);

        builder.HasIndex(u => u.CredentialId).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();
    }
}