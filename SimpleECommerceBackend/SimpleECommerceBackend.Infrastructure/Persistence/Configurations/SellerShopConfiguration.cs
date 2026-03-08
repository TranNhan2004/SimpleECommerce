using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations;

public class SellerShopConfiguration : IEntityTypeConfiguration<SellerShop>
{
    public void Configure(EntityTypeBuilder<SellerShop> builder)
    {
        builder.ToTable("SellerShops");

        builder.Property(ss => ss.Name)
            .IsRequired();

        builder.Property(ss => ss.PhoneNumber)
            .IsRequired()
            .HasMaxLength(CommonConstants.PhoneNumberMaxLength);

        builder.Property(ss => ss.AvatarUrl);

        builder.HasOne(ss => ss.Seller)
            .WithOne()
            .HasForeignKey<SellerShop>(ss => ss.SellerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ss => ss.SellerId);
    }
}