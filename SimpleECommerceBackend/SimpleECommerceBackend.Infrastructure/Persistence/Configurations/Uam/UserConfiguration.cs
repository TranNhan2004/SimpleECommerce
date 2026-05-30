using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Uam;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", DbSchemas.Uam);

        builder.Property(u => u.KeycloakSubjectId)
            .IsRequired();

        builder.Property(u => u.Email)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(UserProfileValidationRules.FirstNameMaxLength);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(UserProfileValidationRules.LastNameMaxLength);

        builder.Property(u => u.NickName)
            .HasMaxLength(UserProfileValidationRules.NickNameMaxLength)
            .HasDefaultValue(null);

        builder.Property(u => u.Sex).IsRequired();
        builder.Property(u => u.Status).IsRequired();
        builder.Property(u => u.BirthDate).HasDefaultValue(null);
        builder.Property(u => u.AvatarUrl).HasDefaultValue(null);

        builder.HasIndex(u => u.KeycloakSubjectId)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter($"[{nameof(User.Status)}] <> {(int)UserStatus.Archived}");
    }
}
