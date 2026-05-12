using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class ProductImageTests
{
    [Fact]
    public void Create_ShouldCreateProductVariantImage_WhenInputIsValid()
    {
        var image = new ProductVariantImage
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            ImageUrl = "  https://example.com/image.jpg  ",
            DisplayOrder = 1,
            Description = "  Front view  "
        };

        image.ImageUrl.Should().Be("https://example.com/image.jpg");
        image.DisplayOrder.Should().Be(1);
        image.Description.Should().Be("Front view");
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenImageUrlIsBlank()
    {
        var action = () => new ProductVariantImage
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            ImageUrl = " ",
            DisplayOrder = 1
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductVariantImageErrorCodes.ImageUrlRequired);
    }

    [Fact]
    public void SetDisplayOrder_ShouldThrowValidationException_WhenDisplayOrderIsNegative()
    {
        var image = EntityTestData.CreateProductVariantImage();
        var action = () => image.DisplayOrder = -1;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductVariantImageErrorCodes.DisplayOrderCannotBeNegative);
    }

    [Fact]
    public void SetDescription_ShouldThrowValidationException_WhenDescriptionExceedsMaxLength()
    {
        var image = EntityTestData.CreateProductVariantImage();
        var description = new string('a', ProductVariantImageValidationRules.DescriptionMaxLength + 1);
        var action = () => image.Description = description;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductVariantImageErrorCodes.DescriptionMaxLengthExceeded);
    }

    [Fact]
    public void SetDescription_ShouldAllowNull()
    {
        var image = EntityTestData.CreateProductVariantImage(description: "Front view");

        image.Description = null;

        image.Description.Should().BeNull();
    }
}
