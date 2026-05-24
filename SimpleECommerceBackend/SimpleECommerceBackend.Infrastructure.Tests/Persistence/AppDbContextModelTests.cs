using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Entities.Uam;
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

    [Fact]
    public void User_ShouldMapToUamSchema()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=localhost;Database=ModelValidation;User Id=sa;Password=StrongPassword!123;TrustServerCertificate=True;")
            .Options;

        using var context = new AppDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(User));

        entityType.Should().NotBeNull();
        entityType!.GetSchema().Should().Be(DbSchemas.Uam);
        entityType.GetTableName().Should().Be("Users");
    }

    [Fact]
    public void Category_AdminForeignKey_ShouldReferenceUamUsers()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=localhost;Database=ModelValidation;User Id=sa;Password=StrongPassword!123;TrustServerCertificate=True;")
            .Options;

        using var context = new AppDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(Category));

        entityType.Should().NotBeNull();

        var adminForeignKey = entityType!.GetForeignKeys()
            .Single(foreignKey => foreignKey.Properties.Select(property => property.Name).SequenceEqual(["AdminId"]));

        adminForeignKey.PrincipalEntityType.ClrType.Should().Be(typeof(User));
        adminForeignKey.PrincipalEntityType.GetSchema().Should().Be(DbSchemas.Uam);
        adminForeignKey.PrincipalEntityType.GetTableName().Should().Be("Users");
    }
}
