using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations;

public sealed class TranslationEntryConfiguration : IEntityTypeConfiguration<TranslationEntry>
{
    public void Configure(EntityTypeBuilder<TranslationEntry> builder)
    {
        builder.ToTable("Translations", DbSchemas.Translation);

        builder.Property(x => x.EntityName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.FieldName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Locale)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.HasIndex(x => new { x.EntityName, x.FieldName, x.RowId, x.Locale })
            .IsUnique();
    }
}
