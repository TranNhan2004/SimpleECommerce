using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Tests.Persistence;

public class AppDbContextModelTests
{
    [Fact]
    public void ProductVariantPrice_ShouldUseSingleForeignKeyToProductVariant()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=localhost;Database=ModelValidation;User Id=sa;Password=StrongPassword!123;TrustServerCertificate=True;")
            .Options;

        using var context = new AppDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(ProductVariantPrice));

        entityType.Should().NotBeNull();
        entityType!.FindProperty("ProductVariantId1").Should().BeNull();

        var foreignKeys = entityType.GetForeignKeys()
            .Where(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(ProductVariant))
            .ToList();

        foreignKeys.Should().HaveCount(1);
        foreignKeys[0].Properties.Select(property => property.Name).Should().Equal("ProductVariantId");
    }
}
