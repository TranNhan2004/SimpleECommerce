using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class OrderItem : EntityBase
{
    public OrderItem()
    {
    }

    // private OrderItem(Guid productId, Guid orderId, int quantity, Money currentPrice)
    // {
    //     Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
    //     ProductId = productId;
    //     OrderId = orderId;
    //     Quantity = quantity;
    //     CurrentPrice = currentPrice;
    // }

    private Guid _productVariantId;
    private Guid _orderId;
    private int _quantity;
    private Money _currentPrice;

    public Guid ProductVariantId
    {
        get => _productVariantId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    OrderItemErrorCodes.ProductVariantIdRequired,
                    "Product variant ID is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ProductVariantId"
                    }
                );

            _productVariantId = value;
        }
    }

    public ProductVariant? ProductVariant { get; private set; }

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
    public Money GetLineTotal()
    {
        return new Money(CurrentPrice.Amount * Quantity, CurrentPrice.Currency);
    }
}
