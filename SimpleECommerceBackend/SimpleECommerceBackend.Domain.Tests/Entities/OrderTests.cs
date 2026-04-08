using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void Create_ShouldCreateOrder_WhenInputIsValid()
    {
        var order = CreateOrder();

        order.Code.Should().Be("ORD-001");
        order.Note.Should().Be("Please deliver fast");
        order.ShopName.Should().Be("Book Shop");
        order.Status.Should().Be(OrderStatus.PendingPayment);
        order.TotalPrice.Should().Be(order.ShippingFee);
    }

    [Fact]
    public void Create_ShouldTrimShopNameAndRecipientFields()
    {
        var order = Order.Create(
            "ORD-001",
            "note",
            EntityTestData.CreateMoney(),
            OrderStatus.PendingPayment,
            "  Book Shop  ",
            EntityTestData.CreateAddress(),
            "  Nhan  ",
            " 0987654321 ",
            EntityTestData.CreateAddress("456 Le Loi", "Ward 2", "Da Nang"),
            Guid.NewGuid(),
            Guid.NewGuid());

        order.ShopName.Should().Be("Book Shop");
        order.RecipientName.Should().Be("Nhan");
        order.RecipientPhoneNumber.Should().Be("0987654321");
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenCodeIsBlank()
    {
        var action = () => Order.Create(
            " ",
            null,
            EntityTestData.CreateMoney(),
            OrderStatus.PendingPayment,
            "Shop",
            EntityTestData.CreateAddress(),
            "Nhan",
            "0987654321",
            EntityTestData.CreateAddress(),
            Guid.NewGuid(),
            Guid.NewGuid());

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(OrderErrorCode.CodeRequired);
    }

    [Fact]
    public void Pickup_ShouldChangeStatus_WhenOrderIsPendingPayment()
    {
        var order = CreateOrder();

        order.Pickup();

        order.Status.Should().Be(OrderStatus.ReadyToPickup);
    }

    [Fact]
    public void Deliver_ShouldThrowValidationException_WhenOrderHasNotReachedAwaitingConfirmation()
    {
        var order = CreateOrder();
        var action = () => order.Deliver();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(OrderErrorCode.DeliverNotAllowed);
    }

    [Fact]
    public void Return_ShouldChangeStatus_WhenOrderIsAwaitingConfirmation()
    {
        var order = CreateOrder();
        order.Pickup();
        order.Ship();
        order.AwaitConfirmation();

        order.Return();

        order.Status.Should().Be(OrderStatus.Returned);
    }

    [Fact]
    public void Expire_ShouldSetStatusAndExpiredAt_WhenOrderIsPendingPayment()
    {
        var order = CreateOrder();
        var before = DateTimeOffset.UtcNow.AddDays(1).AddSeconds(-5);

        order.Expire();

        order.Status.Should().Be(OrderStatus.Expired);
        order.ExpiredAt.Should().NotBeNull();
        order.ExpiredAt.Should().BeAfter(before);
    }

    private static Order CreateOrder()
    {
        return Order.Create(
            "ORD-001",
            "Please deliver fast",
            EntityTestData.CreateMoney(),
            OrderStatus.PendingPayment,
            "Book Shop",
            EntityTestData.CreateAddress(),
            "Nhan",
            "0987654321",
            EntityTestData.CreateAddress("456 Le Loi", "Ward 2", "Da Nang"),
            Guid.NewGuid(),
            Guid.NewGuid());
    }
}
