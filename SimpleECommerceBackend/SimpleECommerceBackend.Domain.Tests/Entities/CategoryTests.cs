using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_ShouldCreateCategory_WhenInputIsValid()
    {
        var adminId = Guid.NewGuid();

        var category = new Category
        {
            Name = "  Books  ",
            Description = "  All kinds of books  ",
            AdminId = adminId,
            Status = CategoryStatus.Active
        };

        category.Name.Should().Be("Books");
        category.Description.Should().Be("All kinds of books");
        category.AdminId.Should().Be(adminId);
        category.Status.Should().Be(CategoryStatus.Active);
    }

    [Fact]
    public void Create_ShouldAllowNullDescription()
    {
        var category = new Category
        {
            Name = "Books",
            Description = null,
            AdminId = Guid.NewGuid(),
            Status = CategoryStatus.Active
        };

        category.Description.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowValidationException_WhenNameIsEmptyOrWhitespace(string name)
    {
        var action = () => new Category
        {
            Name = name,
            Description = "desc",
            AdminId = Guid.NewGuid(),
            Status = CategoryStatus.Active
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CategoryErrorCodes.NameRequired);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var name = new string('a', CategoryValidationRules.NameMaxLength + 1);
        var action = () => new Category
        {
            Name = name,
            Description = "desc",
            AdminId = Guid.NewGuid(),
            Status = CategoryStatus.Active
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CategoryErrorCodes.NameMaxLengthExceeded);
    }

    [Fact]
    public void Deactivate_ShouldChangeStatus_WhenCategoryIsActive()
    {
        var category = CreateCategory();

        category.Deactivate();

        category.Status.Should().Be(CategoryStatus.Inactive);
    }

    [Fact]
    public void Activate_ShouldThrowValidationException_WhenCategoryIsArchived()
    {
        var category = CreateCategory();
        category.Archive();
        var action = () => category.Activate();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CategoryErrorCodes.ActivateNotAllowed);
    }

    [Fact]
    public void Archive_ShouldThrowValidationException_WhenCategoryIsAlreadyArchived()
    {
        var category = CreateCategory();
        category.Archive();
        var action = () => category.Archive();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CategoryErrorCodes.AlreadyArchived);
    }

    private static Category CreateCategory()
    {
        return new Category
        {
            Name = "Books",
            Description = "Description",
            AdminId = Guid.NewGuid(),
            Status = CategoryStatus.Active
        };
    }
}
