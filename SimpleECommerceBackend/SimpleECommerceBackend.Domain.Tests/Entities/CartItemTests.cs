using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class CartItemTests
{
    [Fact]
    public void Create_ShouldCreateCartItem_WhenInputIsValid()
    {
        var productVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var cartId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();

        var cartItem = new CartItem
        {
            ProductVariantId = productVariantId,
            CartId = cartId,
            Quantity = 2
        };

        cartItem.ProductVariantId.Should().Be(productVariantId);
        cartItem.CartId.Should().Be(cartId);
        cartItem.Quantity.Should().Be(2);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenProductVariantIdIsEmpty()
    {
        var action = () => new CartItem
        {
            ProductVariantId = Guid.Empty,
            CartId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Quantity = 1
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CartItemErrorCodes.ProductVariantIdRequired);
    }

    [Fact]
    public void SetQuantity_ShouldThrowValidationException_WhenQuantityExceedsMaximum()
    {
        var cartItem = new CartItem
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            CartId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Quantity = 1
        };
        var action = () => cartItem.Quantity = CartValidationRules.MaxQuantityPerItem + 1;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CartItemErrorCodes.QuantityCannotExceed);
    }

    [Fact]
    public void IncreaseQuantity_ShouldUpdateQuantity_WhenAmountIsValid()
    {
        var cartItem = new CartItem
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            CartId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Quantity = 2
        };

        cartItem.IncreaseQuantity(3);

        cartItem.Quantity.Should().Be(5);
    }

    [Fact]
    public void DecreaseQuantity_ShouldThrowValidationException_WhenResultIsNotPositive()
    {
        var cartItem = new CartItem
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            CartId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Quantity = 2
        };
        var action = () => cartItem.DecreaseQuantity(2);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CartItemErrorCodes.QuantityMustBeGreaterThanZero);
    }
}
