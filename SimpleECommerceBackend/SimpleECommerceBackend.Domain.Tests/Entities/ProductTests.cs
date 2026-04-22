using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Create_ShouldCreateProduct_WhenInputIsValid()
    {
        var product = CreateProduct();

        product.Name.Should().Be("Book");
        product.Description.Should().Be("A good book");
        product.CurrentPrice.Should().Be(EntityTestData.CreateMoney());
        product.Status.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenNameIsBlank()
    {
        var action = () => Product.Create(
            " ",
            "A good book",
            EntityTestData.CreateMoney(),
            Guid.NewGuid(),
            Guid.NewGuid());

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductErrorCodes.NameRequired);
    }

    [Fact]
    public void Activate_ShouldChangeStatus_WhenProductIsDraft()
    {
        var product = CreateProduct();

        product.Activate();

        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void Hide_ShouldThrowValidationException_WhenProductIsHidden()
    {
        var product = CreateProduct();
        product.Hide();
        var action = () => product.Hide();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductErrorCodes.HideNotAllowed);
    }

    [Fact]
    public void AddImage_ShouldAppendImage()
    {
        var product = CreateProduct();
        var image = EntityTestData.CreateProductImage();

        product.AddImage(image);

        product.ProductImages.Should().ContainSingle().Which.Should().Be(image);
    }

    [Fact]
    public void ChangeImage_ShouldUpdateExistingImage_WhenImageExists()
    {
        var product = CreateProduct();
        var imageId = Guid.NewGuid();
        var existingImage = EntityTestData.CreateProductImage(description: "Old");
        var updatedImage = EntityTestData.CreateProductImage(displayOrder: 3, isDisplayed: false, description: "New");
        EntityTestData.AssignId(existingImage, imageId);
        EntityTestData.AssignId(updatedImage, imageId);
        product.AddImage(existingImage);

        product.ChangeImage(updatedImage);

        existingImage.Description.Should().Be("New");
        existingImage.DisplayOrder.Should().Be(3);
        existingImage.IsDisplayed.Should().BeFalse();
    }

    [Fact]
    public void RemoveImage_ShouldThrowValidationException_WhenImageDoesNotExist()
    {
        var product = CreateProduct();
        var action = () => product.RemoveImage(Guid.NewGuid());

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductErrorCodes.ImageNotFound);
    }

    private static Product CreateProduct()
    {
        return Product.Create(
            "Book",
            "A good book",
            EntityTestData.CreateMoney(),
            Guid.NewGuid(),
            Guid.NewGuid());
    }
}
