using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.AuditTracking;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.AuditTracking;

public class AuditConfiguration : IEntityTypeConfiguration<Audit>
{
    public void Configure(EntityTypeBuilder<Audit> builder)
    {
        builder.ToTable("Audits", DbSchemas.AuditTracking);

        builder.Property(a => a.EntityId)
            .IsRequired();

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.OperationType)
            .IsRequired();

        builder.Property(a => a.TraceId)
            .IsRequired()
            .HasMaxLength(127);

        builder.Property(a => a.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(a => a.OldValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValues)
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.AuditedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.AuditedAt)
            .IsRequired();

        builder.HasIndex(a => new { a.EntityName, a.EntityId });
        builder.HasIndex(a => a.TraceId);
        builder.HasIndex(a => a.IpAddress);
        builder.HasIndex(a => a.AuditedAt);
        builder.HasIndex(a => a.AuditedBy);
    }
}
