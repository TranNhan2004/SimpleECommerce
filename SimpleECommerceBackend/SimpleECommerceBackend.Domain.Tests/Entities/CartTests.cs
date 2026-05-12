using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class CartTests
{
    [Fact]
    public void Create_ShouldCreateCart_WhenInputIsValid()
    {
        var customerId = Guid.NewGuid();

        var cart = new Cart
        {
            CustomerId = customerId
        };

        cart.CustomerId.Should().Be(customerId);
        cart.CartItems.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenCustomerIdIsEmpty()
    {
        var action = () => new Cart
        {
            CustomerId = Guid.Empty
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CartErrorCodes.CustomerIdRequired);
    }

    [Fact]
    public void AddCartItem_ShouldAppendItem()
    {
        var cart = new Cart
        {
            CustomerId = Guid.NewGuid()
        };
        var cartItem = new CartItem
        {
            ProductVariantId = Guid.NewGuid(),
            CartId = Guid.NewGuid(),
            Quantity = 2
        };

        cart.AddCartItem(cartItem);

        cart.CartItems.Should().ContainSingle().Which.Should().Be(cartItem);
    }
}
