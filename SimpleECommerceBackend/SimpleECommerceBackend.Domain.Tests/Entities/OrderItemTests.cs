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
        var productVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var orderId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var quantity = 5;
        var currentPrice = new Money(100000, "VND");
        var orderItem = new OrderItem
        {
            ProductVariantId = productVariantId,
            OrderId = orderId,
            Quantity = quantity,
            CurrentPrice = currentPrice
        };

        orderItem.ProductVariantId.Should().Be(productVariantId);
        orderItem.OrderId.Should().Be(orderId);
        orderItem.Quantity.Should().Be(quantity);
        orderItem.CurrentPrice.Should().Be(currentPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldThrow_WhenQuantityIsNotPositive(int quantity)
    {
        var productVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var orderId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var currentPrice = new Money(100000, "VND");
        var act = () => new OrderItem
        {
            ProductVariantId = productVariantId,
            OrderId = orderId,
            Quantity = quantity,
            CurrentPrice = currentPrice
        };

        act.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(OrderItemErrorCodes.QuantityMustBeGreaterThanZero);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenOrderIdIsEmpty()
    {
        var action = () => new OrderItem
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            OrderId = Guid.Empty,
            Quantity = 1,
            CurrentPrice = new Money(100000, "VND")
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(OrderItemErrorCodes.OrderIdRequired);
    }

    [Fact]
    public void SetCurrentPrice_ShouldThrowValidationException_WhenAmountIsNegative()
    {
        var orderItem = new OrderItem
        {
            ProductVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            OrderId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            Quantity = 1,
            CurrentPrice = new Money(100000, "VND")
        };
        var action = () => orderItem.CurrentPrice = new Money(-1, "VND");

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be("Money_AmountCannotBeNegative");
    }

    [Fact]
    public void GetLineTotal_ShouldCalculateCorrectly()
    {
        var productVariantId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var orderId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
        var quantity = 5;
        var currentPrice = new Money(100000, "VND");
        var orderItem = new OrderItem
        {
            ProductVariantId = productVariantId,
            OrderId = orderId,
            Quantity = quantity,
            CurrentPrice = currentPrice
        };
        var lineTotal = orderItem.GetLineTotal();

        lineTotal.Amount.Should().Be(500000);
        lineTotal.Currency.Should().Be("VND");
    }
}
