using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class OrderShippingAddressConfiguration : IEntityTypeConfiguration<OrderShippingAddress>
{
    public void Configure(EntityTypeBuilder<OrderShippingAddress> builder)
    {
        builder.ToTable("OrderShippingAddresses", "dbo");

        builder.Property(osa => osa.RecipientName)
            .IsRequired()
            .HasMaxLength(AddressConstants.RecipientNameMaxLength);

        builder.Property(osa => osa.PhoneNumber)
            .IsRequired()
            .HasMaxLength(AddressConstants.PhoneNumberMaxLength);

        builder.Property(osa => osa.AddressLine)
            .IsRequired()
            .HasMaxLength(AddressConstants.AddressLineMaxLength);

        builder.Property(osa => osa.Ward);
        builder.Property(osa => osa.Province);
    }
}