using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class CustomerShippingAddressConfiguration : IEntityTypeConfiguration<CustomerShippingAddress>
{
    public void Configure(EntityTypeBuilder<CustomerShippingAddress> builder)
    {
        builder.ToTable("CustomerShippingAddresses", "dbo");

        builder.Property(usa => usa.RecipientName)
            .IsRequired()
            .HasMaxLength(AddressConstants.RecipientNameMaxLength);

        builder.Property(usa => usa.PhoneNumber)
            .IsRequired()
            .HasMaxLength(AddressConstants.PhoneNumberMaxLength);

        builder.Property(usa => usa.AddressLine)
            .IsRequired();

        builder.Property(usa => usa.Ward)
            .IsRequired();

        builder.Property(usa => usa.Province)
            .IsRequired();

        builder.Property(usa => usa.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(usa => usa.Customer)
            .WithMany()
            .IsRequired()
            .HasForeignKey(usa => usa.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(usa => usa.CustomerId);
        builder.HasIndex(usa => new { usa.CustomerId, usa.IsDefault })
            .HasFilter(
                $"[{nameof(CustomerShippingAddress.IsDefault)}] = 1 AND [{nameof(CustomerShippingAddress.IsDeleted)}] = 0"
            );
    }
}