using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class SellerWarehouseConfiguration : IEntityTypeConfiguration<SellerWarehouse>
{
    public void Configure(EntityTypeBuilder<SellerWarehouse> builder)
    {
        builder.ToTable("SellerWarehouses", "dbo");
        
        builder.ComplexProperty(sw => sw.FullAddress, address =>
        {
            address.Property(a => a.AddressLine)
                .HasColumnName("AddressLine")
                .IsRequired()
                .HasMaxLength(AddressConstants.AddressLineMaxLength);

            address.Property(a => a.Province)
                .HasColumnName("Province")
                .IsRequired();

            address.Property(a => a.Ward)
                .HasColumnName("Ward")
                .IsRequired();
        });

        builder.HasOne(sw => sw.SellerShop)
            .WithMany(ss => ss.SellerWarehouses)
            .IsRequired()
            .HasForeignKey(sw => sw.SellerShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sw => sw.SellerShopId);
    }
}