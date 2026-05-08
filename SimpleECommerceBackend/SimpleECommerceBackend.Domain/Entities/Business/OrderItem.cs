using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class OrderItem : Entity, ICreatedTrackable, IUpdatedTrackable
{
    public OrderItem()
    {
    }

    private OrderItem(Guid productId, Guid orderId, int quantity, Money currentPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        OrderId = orderId;
        Quantity = quantity;
        CurrentPrice = currentPrice;
    }

    private Guid _productId;
    private Guid _orderId;
    private int _quantity;
    private Money _currentPrice;

    public Guid ProductId
    {
        get => _productId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    OrderItemErrorCodes.ProductIdRequired,
                    "Product ID is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ProductId"
                    }
                );

            _productId = value;
        }
    }

    public Product? Product { get; private set; }

    public Guid OrderId
    {
        get => _orderId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    OrderItemErrorCodes.OrderIdRequired,
                    "Order ID is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "OrderId"
                    }
                );

            _orderId = value;
        }
    }

    public Order? Order { get; private set; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
                throw new ValidationException(
                    OrderItemErrorCodes.QuantityMustBeGreaterThanZero,
                    "Quantity must be greater than zero",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Quantity"
                    }
                );

            _quantity = value;
        }
    }

    public Money CurrentPrice
    {
        get => _currentPrice;
        set
        {
            if (value.Amount < 0)
                throw new ValidationException(
                    OrderItemErrorCodes.AmountCannotBeNegative,
                    "Amount cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Amount"
                    }
                );

            _currentPrice = value;
        }
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public Money GetLineTotal()
    {
        return new Money(CurrentPrice.Amount * Quantity, CurrentPrice.Currency);
    }
}
