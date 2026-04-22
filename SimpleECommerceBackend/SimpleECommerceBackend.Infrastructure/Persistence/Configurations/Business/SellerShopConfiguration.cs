using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class SellerShopConfiguration : IEntityTypeConfiguration<SellerShop>
{
    public void Configure(EntityTypeBuilder<SellerShop> builder)
    {
        builder.ToTable("SellerShops", DbSchemas.Business);

        builder.Property(ss => ss.Name)
            .IsRequired();

        builder.Property(ss => ss.PhoneNumber)
            .IsRequired()
            .HasMaxLength(CommonValidationRules.PhoneNumberMaxLength);

        builder.Property(ss => ss.AvatarUrl);

        builder.HasOne(ss => ss.Seller)
            .WithOne()
            .HasForeignKey<SellerShop>(ss => ss.SellerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ss => ss.SellerId);
    }
}