using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class ReviewResponseConfiguration : IEntityTypeConfiguration<ReviewResponse>
{
    public void Configure(EntityTypeBuilder<ReviewResponse> builder)
    {
        builder.ToTable("ReviewResponses", DbSchemas.Business);

        builder.Property(rr => rr.Comment)
            .IsRequired()
            .HasMaxLength(ReviewResponseValidationRules.CommentMaxLength);

        builder.HasOne(rr => rr.Review)
            .WithMany(r => r.Responses)
            .IsRequired()
            .HasForeignKey(rr => rr.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rr => rr.FromUser)
            .WithMany()
            .IsRequired()
            .HasForeignKey(rr => rr.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rr => rr.ToUser)
            .WithMany()
            .IsRequired()
            .HasForeignKey(rr => rr.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rr => rr.ReviewId);
        builder.HasIndex(rr => rr.FromUserId);
        builder.HasIndex(rr => rr.ToUserId);
    }
}
