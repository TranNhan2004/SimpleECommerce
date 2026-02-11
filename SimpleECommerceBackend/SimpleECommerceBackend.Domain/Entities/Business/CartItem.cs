using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class CartItem : EntityBase, ICreatedTime, IUpdatedTime
{
    private CartItem()
    {
    }

    private CartItem(Guid productId, Guid cartId, int quantity)
    {
        SetProductId(productId);
        SetCartId(cartId);
        SetQuantity(quantity);
    }

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }

    public Guid CartId { get; private set; }
    public Cart? Cart { get; private set; }

    public int Quantity { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static CartItem Create(Guid productId, Guid cartId, int quantity)
    {
        return new CartItem(productId, cartId, quantity);
    }

    private void SetProductId(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product ID is required");

        ProductId = productId;
    }

    private void SetCartId(Guid cartId)
    {
        if (cartId == Guid.Empty)
            throw new DomainException("Cart ID is required");

        CartId = cartId;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (quantity > CartConstants.MaxQuantityPerItem)
            throw new DomainException($"Quantity cannot exceed {CartConstants.MaxQuantityPerItem}");

        Quantity = quantity;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be positive");

        SetQuantity(Quantity + amount);
    }

    public void DecreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be positive");

        SetQuantity(Quantity - amount);
    }
}