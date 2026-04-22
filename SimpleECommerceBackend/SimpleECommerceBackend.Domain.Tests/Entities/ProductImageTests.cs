using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class ProductImageTests
{
    [Fact]
    public void Create_ShouldCreateProductImage_WhenInputIsValid()
    {
        var image = ProductImage.Create("  https://example.com/image.jpg  ", 1, description: "  Front view  ");

        image.ImageUrl.Should().Be("https://example.com/image.jpg");
        image.DisplayOrder.Should().Be(1);
        image.IsDisplayed.Should().BeTrue();
        image.Description.Should().Be("Front view");
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenImageUrlIsBlank()
    {
        var action = () => ProductImage.Create(" ", 1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductImageErrorCodes.ImageUrlRequired);
    }

    [Fact]
    public void SetDisplayOrder_ShouldThrowValidationException_WhenDisplayOrderIsNegative()
    {
        var image = ProductImage.Create("https://example.com/image.jpg", 1);
        var action = () => image.SetDisplayOrder(-1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductImageErrorCodes.DisplayOrderCannotBeNegative);
    }

    [Fact]
    public void SetDescription_ShouldThrowValidationException_WhenDescriptionExceedsMaxLength()
    {
        var image = ProductImage.Create("https://example.com/image.jpg", 1);
        var description = new string('a', ProductImageValidationRules.DescriptionMaxLength + 1);
        var action = () => image.SetDescription(description);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductImageErrorCodes.DescriptionMaxLengthExceeded);
    }

    [Fact]
    public void SetDescription_ShouldAllowNull()
    {
        var image = ProductImage.Create("https://example.com/image.jpg", 1, description: "Front view");

        image.SetDescription(null);

        image.Description.Should().BeNull();
    }
}
