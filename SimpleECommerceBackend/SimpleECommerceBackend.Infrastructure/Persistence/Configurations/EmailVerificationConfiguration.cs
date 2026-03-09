using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations;

public class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
{
    public void Configure(EntityTypeBuilder<EmailVerification> builder)
    {
        builder.ToTable("EmailVerifications");

        builder.Property(emailVerification => emailVerification.Email)
            .IsRequired()
            .HasMaxLength(CredentialConstants.EmailMaxLength);

        builder.Property(emailVerification => emailVerification.TokenHash)
            .IsRequired()
            .HasMaxLength(EmailVerificationConstants.TokenHashLength);

        builder.Property(emailVerification => emailVerification.ExpiresAt)
            .IsRequired();

        builder.Property(emailVerification => emailVerification.ConfirmedAt);

        builder.HasOne(emailVerification => emailVerification.User)
            .WithMany()
            .IsRequired()
            .HasForeignKey(emailVerification => emailVerification.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(emailVerification => emailVerification.TokenHash)
            .IsUnique();

        builder.HasIndex(emailVerification => emailVerification.UserId);
        builder.HasIndex(emailVerification => emailVerification.ExpiresAt);
    }
}