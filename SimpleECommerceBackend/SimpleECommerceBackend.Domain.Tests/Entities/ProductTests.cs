using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Create_ShouldCreateProduct_WhenInputIsValid()
    {
        var product = CreateProduct();

        product.Name.Should().Be("Book");
        product.Description.Should().Be("A good book");
        product.CategoryId.Should().NotBe(Guid.Empty);
        product.SellerId.Should().NotBe(Guid.Empty);
        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenNameIsBlank()
    {
        var action = () => new Product
        {
            Name = " ",
            Description = "A good book",
            CategoryId = UuidUtils.CreateV7(),
            SellerId = UuidUtils.CreateV7()
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductErrorCodes.NameRequired);
    }

    [Fact]
    public void SetAverageRating_ShouldThrowValidationException_WhenValueIsOutOfRange()
    {
        var product = CreateProduct();
        var action = () => product.AverageRating = 6;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductErrorCodes.AverageRatingOutOfRange);
    }

    [Fact]
    public void AddVariant_ShouldAppendVariant()
    {
        var product = CreateProduct();
        var variant = CreateProductVariant();

        product.AddVariant(variant);

        product.ProductVariants.Should().ContainSingle().Which.Should().Be(variant);
    }

    [Fact]
    public void AddReview_ShouldAppendReview()
    {
        var product = CreateProduct();
        var review = new Review
        {
            ProductId = UuidUtils.CreateV7(),
            CustomerId = UuidUtils.CreateV7(),
            Rating = 5,
            Comment = "Excellent"
        };

        product.AddReview(review);

        product.Reviews.Should().ContainSingle().Which.Should().Be(review);
    }

    [Fact]
    public void ProductVariant_ChangeImage_ShouldUpdateExistingImage_WhenImageExists()
    {
        var variant = CreateProductVariant();
        var imageId = UuidUtils.CreateV7();
        var existingImage = EntityTestData.CreateProductVariantImage(description: "Old");
        var updatedImage = EntityTestData.CreateProductVariantImage(displayOrder: 3, isDisplayed: false, description: "New");
        EntityTestData.AssignId(existingImage, imageId);
        EntityTestData.AssignId(updatedImage, imageId);
        variant.AddImage(existingImage);

        variant.ChangeImage(updatedImage);

        existingImage.Description.Should().Be("New");
        existingImage.DisplayOrder.Should().Be(3);
        existingImage.IsDisplayed.Should().BeFalse();
    }

    [Fact]
    public void ProductVariant_RemoveImage_ShouldThrowValidationException_WhenImageDoesNotExist()
    {
        var variant = CreateProductVariant();
        var action = () => variant.RemoveImage(UuidUtils.CreateV7());

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(ProductVariantErrorCodes.ImageNotFound);
    }

    private static Product CreateProduct()
    {
        return new Product
        {
            Name = "Book",
            Description = "A good book",
            CategoryId = UuidUtils.CreateV7(),
            SellerId = UuidUtils.CreateV7(),
            AverageRating = 4.5,
            TotalRatings = 10
        };
    }

    private static ProductVariant CreateProductVariant()
    {
        return new ProductVariant
        {
            ProductId = UuidUtils.CreateV7(),
            Name = "Hardcover",
            Description = "Hardcover edition",
            CurrentPrice = EntityTestData.CreateMoney(),
            TotalInStock = 10,
            DefaultImageUrl = "https://example.com/variant.jpg",
            Status = ProductInvariantStatus.Draft
        };
    }
}
