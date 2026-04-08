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

        var cart = Cart.Create(customerId);

        cart.CustomerId.Should().Be(customerId);
        cart.CartItems.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenCustomerIdIsEmpty()
    {
        var action = () => Cart.Create(Guid.Empty);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(CartErrorCode.CustomerIdRequired);
    }

    [Fact]
    public void AddCartItem_ShouldAppendItem()
    {
        var cart = Cart.Create(Guid.NewGuid());
        var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 2);

        cart.AddCartItem(cartItem);

        cart.CartItems.Should().ContainSingle().Which.Should().Be(cartItem);
    }
}
