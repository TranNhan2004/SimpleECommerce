using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class OrderItemTests
{
    [Fact]
    public void Create_ShouldCreateOrderItem_WhenInputIsValid()
    {
        var productId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var quantity = 5;
        var currentPrice = new Money(100000, "VND");
        var orderItem = OrderItem.Create(productId, orderId, quantity, currentPrice);

        orderItem.ProductId.Should().Be(productId);
        orderItem.OrderId.Should().Be(orderId);
        orderItem.Quantity.Should().Be(quantity);
        orderItem.CurrentPrice.Should().Be(currentPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldThrow_WhenQuantityIsNotPositive(int quantity)
    {
        var productId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var currentPrice = new Money(100000, "VND");
        var act = () => OrderItem.Create(productId, orderId, quantity, currentPrice);

        act.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(OrderItemErrorCode.QuantityMustBeGreaterThanZero);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenOrderIdIsEmpty()
    {
        var action = () => OrderItem.Create(Guid.NewGuid(), Guid.Empty, 1, new Money(100000, "VND"));

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(OrderItemErrorCode.OrderIdRequired);
    }

    [Fact]
    public void SetCurrentPrice_ShouldThrowValidationException_WhenAmountIsNegative()
    {
        var orderItem = OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), 1, new Money(100000, "VND"));
        var action = () => orderItem.SetCurrentPrice(new Money(-1, "VND"));

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be("Money_AmountCannotBeNegative");
    }

    [Fact]
    public void GetLineTotal_ShouldCalculateCorrectly()
    {
        var productId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var quantity = 5;
        var currentPrice = new Money(100000, "VND");
        var orderItem = OrderItem.Create(productId, orderId, quantity, currentPrice);
        var lineTotal = orderItem.GetLineTotal();

        lineTotal.Amount.Should().Be(500000);
        lineTotal.Currency.Should().Be("VND");
    }
}
