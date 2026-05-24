using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class CustomerShippingAddressConfiguration : IEntityTypeConfiguration<CustomerShippingAddress>
{
    public void Configure(EntityTypeBuilder<CustomerShippingAddress> builder)
    {
        builder.ToTable("CustomerShippingAddresses", DbSchemas.Business);

        builder.Property(csa => csa.RecipientName)
            .IsRequired()
            .HasMaxLength(ShippingAddressValidationRules.RecipientNameMaxLength);

        builder.Property(csa => csa.RecipientPhoneNumber)
            .IsRequired()
            .HasMaxLength(CommonValidationRules.PhoneNumberMaxLength);

        builder.ComplexProperty(csa => csa.RecipientAddress, address =>
        {
            address.Property(a => a.AddressLine)
                .HasColumnName("RecipientAddressLine")
                .IsRequired()
                .HasMaxLength(AddressValidationRules.AddressLineMaxLength);

            address.Property(a => a.Province)
                .HasColumnName("RecipientProvince")
                .IsRequired();

            address.Property(a => a.Ward)
                .HasColumnName("RecipientWard")
                .IsRequired();
        });

        builder.Property(csa => csa.IsDefault)
            .IsRequired();

        builder.HasOne(csa => csa.Customer)
            .WithMany()
            .IsRequired()
            .HasForeignKey(csa => csa.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(csa => csa.CustomerId);
    }
}