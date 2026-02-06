using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.UnitTests.Entities.Business;

public class CategoryTests
{
    // ---------- Happy path ----------

    [Fact]
    public void Create_ShouldCreateCategory_WhenInputIsValid()
    {
        // Act
        var adminId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
        var category = Category.Create(
            "Books",
            "All kinds of books",
            adminId
        );

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be("Books");
        category.Description.Should().Be("All kinds of books");
        category.AdminId.Should().Be(adminId);
    }

    [Fact]
    public void Create_ShouldTrimNameAndDescription()
    {
        // Act
        var adminId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
        var category = Category.Create(
            "  Books  ",
            "  Description  ",
            adminId
        );

        // Assert
        category.Name.Should().Be("Books");
        category.Description.Should().Be("Description");
    }

    [Fact]
    public void Create_ShouldAllowNullDescription()
    {
        // Act
        var adminId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
        var category = Category.Create("Books", null, adminId);

        // Assert
        category.Description.Should().BeNull();
    }

    // ---------- Name validation ----------

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenNameIsEmptyOrWhitespace(string name)
    {
        // Act
        var adminId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
        var act = () => Category.Create(name, "desc", adminId);

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be("Name is required");
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameExceedsMaxLength()
    {
        // Arrange
        var adminId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
        var name = new string('a', CategoryConstants.NameMaxLength + 1);

        // Act
        var act = () => Category.Create(name, "desc", adminId);

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be($"Name cannot exceed {CategoryConstants.NameMaxLength} characters");
    }

    // ---------- Description validation ----------
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenDescriptionIsEmptyOrWhitespace(string description)
    {
        // Act
        var adminId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
        var act = () => Category.Create("name", description, adminId);

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be("Description is not blank");
    }


    [Fact]
    public void Create_ShouldThrow_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var adminId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
        var description =
            new string('a', CategoryConstants.DescriptionMaxLength + 1);

        // Act
        var act = () => Category.Create("Books", description, adminId);

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should()
            .Be($"Description cannot exceed {CategoryConstants.DescriptionMaxLength} characters");
    }

    // ---------- AdminId validation ----------
    [Fact]
    public void Create_ShouldThrow_WhenAdminIdIsEmpty()
    {
        // Act
        var adminId = Guid.Empty;
        var act = () => Category.Create("Books", null, adminId);

        // Assert
        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should()
            .Be("Admin is required");
    }
}