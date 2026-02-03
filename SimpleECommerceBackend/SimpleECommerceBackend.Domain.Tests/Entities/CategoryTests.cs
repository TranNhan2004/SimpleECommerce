using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class CategoryTests
{
    // ---------- Happy path ----------

    [Fact]
    public void Create_ShouldCreateCategory_WhenInputIsValid()
    {
        // Act
        var category = Category.Create(
            "Books",
            "All kinds of books"
        );

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be("Books");
        category.Description.Should().Be("All kinds of books");
    }

    [Fact]
    public void Create_ShouldTrimNameAndDescription()
    {
        // Act
        var category = Category.Create(
            "  Books  ",
            "  Description  "
        );

        // Assert
        category.Name.Should().Be("Books");
        category.Description.Should().Be("Description");
    }

    [Fact]
    public void Create_ShouldAllowNullDescription()
    {
        // Act
        var category = Category.Create("Books", null);

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
        var act = () => Category.Create(name, "desc");

        // Assert
        var exception = act.Should().Throw<ValidationException>().Which;
        exception.Errors.Should().ContainKey("name");
        exception.Errors["name"].Should().ContainSingle().Which.Should().Be("Category name is required");
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameExceedsMaxLength()
    {
        // Arrange
        var name = new string('a', CategoryConstants.NameMaxLength + 1);

        // Act
        var act = () => Category.Create(name, "desc");

        // Assert
        var exception = act.Should().Throw<ValidationException>().Which;
        exception.Errors.Should().ContainKey("name");
        exception.Errors["name"].Should().ContainSingle().Which.Should()
            .Be($"Category name cannot exceed {CategoryConstants.NameMaxLength} characters");
    }

    // ---------- Description validation ----------
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrow_WhenDescriptionIsEmptyOrWhitespace(string description)
    {
        // Act
        var act = () => Category.Create("name", description);

        // Assert
        var exception = act.Should().Throw<ValidationException>().Which;
        exception.Errors.Should().ContainKey("description");
        exception.Errors["description"].Should().ContainSingle().Which.Should().Be("Category description is not blank");
    }


    [Fact]
    public void Create_ShouldThrow_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var description =
            new string('a', CategoryConstants.DescriptionMaxLength + 1);

        // Act
        var act = () => Category.Create("Books", description);

        // Assert
        var exception = act.Should().Throw<ValidationException>().Which;
        exception.Errors.Should().ContainKey("description");
        exception.Errors["description"].Should().ContainSingle().Which.Should()
            .Be($"Category description cannot exceed {CategoryConstants.DescriptionMaxLength} characters");
    }
}