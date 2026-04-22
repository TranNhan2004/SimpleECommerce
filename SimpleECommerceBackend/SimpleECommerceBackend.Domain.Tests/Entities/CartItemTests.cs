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
        var productId = Guid.NewGuid();
        var cartId = Guid.NewGuid();

        var cartItem = CartItem.Create(productId, cartId, 2);

        cartItem.ProductId.Should().Be(productId);
        cartItem.CartId.Should().Be(cartId);
        cartItem.Quantity.Should().Be(2);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenProductIdIsEmpty()
    {
        var action = () => CartItem.Create(Guid.Empty, Guid.NewGuid(), 1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CartItemErrorCodes.ProductIdRequired);
    }

    [Fact]
    public void SetQuantity_ShouldThrowValidationException_WhenQuantityExceedsMaximum()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 1);
        var action = () => cartItem.SetQuantity(CartValidationRules.MaxQuantityPerItem + 1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CartItemErrorCodes.QuantityCannotExceed);
    }

    [Fact]
    public void IncreaseQuantity_ShouldUpdateQuantity_WhenAmountIsValid()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 2);

        cartItem.IncreaseQuantity(3);

        cartItem.Quantity.Should().Be(5);
    }

    [Fact]
    public void DecreaseQuantity_ShouldThrowValidationException_WhenResultIsNotPositive()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 2);
        var action = () => cartItem.DecreaseQuantity(2);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CartItemErrorCodes.QuantityMustBeGreaterThanZero);
    }
}
