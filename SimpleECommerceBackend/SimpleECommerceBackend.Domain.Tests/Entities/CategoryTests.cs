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

        var category = Category.Create("  Books  ", "  All kinds of books  ", adminId);

        category.Name.Should().Be("Books");
        category.Description.Should().Be("All kinds of books");
        category.AdminId.Should().Be(adminId);
        category.Status.Should().Be(CategoryStatus.Active);
    }

    [Fact]
    public void Create_ShouldAllowNullDescription()
    {
        var category = Category.Create("Books", null, Guid.NewGuid());

        category.Description.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowValidationException_WhenNameIsEmptyOrWhitespace(string name)
    {
        var action = () => Category.Create(name, "desc", Guid.NewGuid());

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CategoryErrorCodes.NameRequired);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenNameExceedsMaxLength()
    {
        var name = new string('a', CategoryValidationRules.NameMaxLength + 1);
        var action = () => Category.Create(name, "desc", Guid.NewGuid());

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CategoryErrorCodes.NameMaxLengthExceeded);
    }

    [Fact]
    public void Deactivate_ShouldChangeStatus_WhenCategoryIsActive()
    {
        var category = Category.Create("Books", "Description", Guid.NewGuid());

        category.Deactivate();

        category.Status.Should().Be(CategoryStatus.Inactive);
    }

    [Fact]
    public void Activate_ShouldThrowValidationException_WhenCategoryIsArchived()
    {
        var category = Category.Create("Books", "Description", Guid.NewGuid());
        category.Archive();
        var action = () => category.Activate();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CategoryErrorCodes.ActivateNotAllowed);
    }

    [Fact]
    public void Archive_ShouldThrowValidationException_WhenCategoryIsAlreadyArchived()
    {
        var category = Category.Create("Books", "Description", Guid.NewGuid());
        category.Archive();
        var action = () => category.Archive();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CategoryErrorCodes.AlreadyArchived);
    }
}
