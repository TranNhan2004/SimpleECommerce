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

        builder.Property(csa => csa.RecipientName)
            .IsRequired()
            .HasMaxLength(ShippingAddressConstants.RecipientNameMaxLength);

        builder.Property(csa => csa.RecipientPhoneNumber)
            .IsRequired()
            .HasMaxLength(CommonConstants.PhoneNumberMaxLength);
        
        builder.ComplexProperty(csa => csa.RecipientAddress, address =>
        {
            address.Property(a => a.AddressLine)
                .HasColumnName("RecipientAddressLine")
                .IsRequired()
                .HasMaxLength(AddressConstants.AddressLineMaxLength);

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