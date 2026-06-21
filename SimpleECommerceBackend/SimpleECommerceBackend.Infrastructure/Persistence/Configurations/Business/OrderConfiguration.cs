using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Extensions;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", DbSchemas.Business);

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(OrderValidationRules.CodeMaxLength);

        builder.Property(o => o.Note)
            .HasMaxLength(OrderValidationRules.NoteMaxLength);

        builder.ComplexProperty(o => o.ShippingFee, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("ShippingAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.ComplexProperty(o => o.TotalPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(o => o.Status)
            .HasEnumStringConversion(50)
            .IsRequired();

        builder.Property(o => o.ShopName)
            .IsRequired()
            .HasMaxLength(SellerShopValidationRules.NameMaxLength);

        builder.ComplexProperty(o => o.WarehouseAddress, address =>
        {
            address.Property(a => a.AddressLine)
                .HasColumnName("WarehouseAddressLine")
                .IsRequired()
                .HasMaxLength(AddressValidationRules.AddressLineMaxLength);

            address.Property(a => a.Province)
                .HasColumnName("WarehouseProvince")
                .IsRequired();

            address.Property(a => a.Ward)
                .HasColumnName("WarehouseWard")
                .IsRequired();
        });

        builder.Property(o => o.RecipientName)
            .IsRequired()
            .HasMaxLength(ShippingAddressValidationRules.RecipientNameMaxLength);

        builder.Property(o => o.RecipientPhoneNumber)
            .IsRequired()
            .HasMaxLength(CommonValidationRules.PhoneNumberMaxLength);

        builder.ComplexProperty(o => o.RecipientAddress, address =>
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

        builder.Property(o => o.ExpiredAt)
            .HasDefaultValue(null);

        builder.HasOne(o => o.Customer)
            .WithMany()
            .IsRequired()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Seller)
            .WithMany()
            .IsRequired()
            .HasForeignKey(o => o.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.Code)
            .IsUnique();

        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.SellerId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}
