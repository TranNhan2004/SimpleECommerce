using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.Property(u => u.Email)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(UserProfileConstants.FirstNameMaxLength);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(UserProfileConstants.LastNameMaxLength);

        builder.Property(u => u.NickName)
            .HasMaxLength(UserProfileConstants.NickNameMaxLength)
            .HasDefaultValue(null);

        builder.Property(u => u.Sex).IsRequired();
        builder.Property(u => u.Status).IsRequired();
        builder.Property(u => u.BirthDate).HasDefaultValue(null);
        builder.Property(u => u.AvatarUrl).HasDefaultValue(null);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter($"[{nameof(UserProfile.Status)}] <> {(int)UserStatus.Archived}");
    }
}