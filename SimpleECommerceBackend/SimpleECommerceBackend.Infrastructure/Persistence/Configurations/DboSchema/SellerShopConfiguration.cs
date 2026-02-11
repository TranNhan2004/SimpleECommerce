using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class SellerShopConfiguration : IEntityTypeConfiguration<SellerShop>
{
    public void Configure(EntityTypeBuilder<SellerShop> builder)
    {
        builder.ToTable("SellerShops", "dbo");
        
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