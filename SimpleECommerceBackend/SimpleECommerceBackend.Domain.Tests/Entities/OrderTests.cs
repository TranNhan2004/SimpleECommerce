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
        var order = new Order
        {
            Code = "ORD-001",
            Note = "note",
            ShippingFee = EntityTestData.CreateMoney(),
            TotalPrice = EntityTestData.CreateMoney(),
            Status = OrderStatus.PendingPayment,
            ShopName = "  Book Shop  ",
            WarehouseAddress = EntityTestData.CreateAddress(),
            RecipientName = "  Nhan  ",
            RecipientPhoneNumber = " 0987654321 ",
            RecipientAddress = EntityTestData.CreateAddress("456 Le Loi", "Ward 2", "Da Nang"),
            CustomerId = Guid.NewGuid(),
            SellerId = Guid.NewGuid()
        };

        order.ShopName.Should().Be("Book Shop");
        order.RecipientName.Should().Be("Nhan");
        order.RecipientPhoneNumber.Should().Be("0987654321");
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenCodeIsBlank()
    {
        var action = () => new Order
        {
            Code = " ",
            Note = null,
            ShippingFee = EntityTestData.CreateMoney(),
            TotalPrice = EntityTestData.CreateMoney(),
            Status = OrderStatus.PendingPayment,
            ShopName = "Shop",
            WarehouseAddress = EntityTestData.CreateAddress(),
            RecipientName = "Nhan",
            RecipientPhoneNumber = "0987654321",
            RecipientAddress = EntityTestData.CreateAddress(),
            CustomerId = Guid.NewGuid(),
            SellerId = Guid.NewGuid()
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(OrderErrorCodes.CodeRequired);
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
            .Which.ErrorCode.Should().Be(OrderErrorCodes.DeliverNotAllowed);
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
        var shippingFee = EntityTestData.CreateMoney();

        return new Order
        {
            Code = "ORD-001",
            Note = "Please deliver fast",
            ShippingFee = shippingFee,
            TotalPrice = shippingFee,
            Status = OrderStatus.PendingPayment,
            ShopName = "Book Shop",
            WarehouseAddress = EntityTestData.CreateAddress(),
            RecipientName = "Nhan",
            RecipientPhoneNumber = "0987654321",
            RecipientAddress = EntityTestData.CreateAddress("456 Le Loi", "Ward 2", "Da Nang"),
            CustomerId = Guid.NewGuid(),
            SellerId = Guid.NewGuid()
        };
    }
}
