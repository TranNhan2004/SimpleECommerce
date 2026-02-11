using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.DboSchema;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", "dbo");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.OrderId)
            .IsRequired();

        builder.ComplexProperty(pp => pp.Money, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(p => p.Method)
            .IsRequired();

        builder.Property(p => p.Provider)
            .HasMaxLength(PaymentConstants.ProviderMaxLength);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.ExternalTransactionId)
            .HasMaxLength(PaymentConstants.ExternalTransactionIdMaxLength);

        builder.HasOne(p => p.Order)
            .WithMany()
            .IsRequired()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.OrderId)
            .IsUnique();

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.ExternalTransactionId);
    }
}