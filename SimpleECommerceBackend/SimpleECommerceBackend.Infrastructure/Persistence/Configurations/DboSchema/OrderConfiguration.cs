using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", "dbo");

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(OrderConstants.CodeMaxLength);

        builder.Property(o => o.Note)
            .HasMaxLength(OrderConstants.NoteMaxLength);

        builder.ComplexProperty(o => o.TotalPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.OrderShippingAddressId);

        builder.HasOne(o => o.Customer)
            .WithMany()
            .IsRequired()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.OrderShippingAddress)
            .WithOne()
            .IsRequired()
            .HasForeignKey<Order>(o => o.OrderShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.Code)
            .IsUnique();

        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}