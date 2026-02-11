using FluentAssertions;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.UnitTests.Domain.Entities.Business;

public class OrderItemTests
{
    [Fact]
    public void Create_ShouldCreateOrderItem_WhenInputIsValid()
    {
        // Arrange
        var productId = Guid.Parse("6984453b-0a40-4f24-833a-ad6649374fce");
        var orderId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
        var quantity = 5;
        var currentPrice = new Money(100000, "VND");

        // Act
        var orderItem = OrderItem.Create(productId, orderId, quantity, currentPrice);

        // Assert
        orderItem.Should().NotBeNull();
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
        // Arrange
        var productId = Guid.Parse("6984453b-0a40-4f24-833a-ad6649374fce");
        var orderId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
        var currentPrice = new Money(100000, "VND");

        // Act
        var act = () => OrderItem.Create(productId, orderId, quantity, currentPrice);

        // Assert
        var exception = act.Should().Throw<DomainException>().Which;
        exception.Message.Should().Be("Quantity must be greater than zero");
    }

    [Fact]
    public void GetLineTotal_ShouldCalculateCorrectly()
    {
        // Arrange
        var productId = Guid.Parse("6984453b-0a40-4f24-833a-ad6649374fce");
        var orderId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
        var quantity = 5;
        var currentPrice = new Money(100000, "VND");
        var orderItem = OrderItem.Create(productId, orderId, quantity, currentPrice);

        // Act
        var lineTotal = orderItem.GetLineTotal();

        // Assert
        lineTotal.Amount.Should().Be(500000);
        lineTotal.Currency.Should().Be("VND");
    }
}