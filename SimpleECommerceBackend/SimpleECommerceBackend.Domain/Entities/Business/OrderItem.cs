using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class OrderItem : EntityBase, ICreatedTime, IUpdatedTime
{
    private OrderItem()
    {
    }

    private OrderItem(Guid productId, Guid orderId, int quantity, Money currentPrice)
    {
        SetProductId(productId);
        SetOrderId(orderId);
        SetQuantity(quantity);
        SetCurrentPrice(currentPrice);
    }

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }

    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }

    public int Quantity { get; private set; }
    public Money CurrentPrice { get; private set; } = new(0, "VND");
    public string Currency { get; private set; } = "VND";

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static OrderItem Create(Guid productId, Guid orderId, int quantity, Money currentPrice)
    {
        return new OrderItem(productId, orderId, quantity, currentPrice);
    }

    private void SetProductId(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product ID is required");

        ProductId = productId;
    }

    private void SetOrderId(Guid orderId)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("Order ID is required");

        OrderId = orderId;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        Quantity = quantity;
    }

    public void SetCurrentPrice(Money currentPrice)
    {
        if (currentPrice.Amount < 0)
            throw new DomainException("Amount cannot be negative");

        CurrentPrice = currentPrice;
        Currency = currentPrice.Currency;
    }

    public Money GetLineTotal()
    {
        return new Money(CurrentPrice.Amount * Quantity, Currency);
    }
}