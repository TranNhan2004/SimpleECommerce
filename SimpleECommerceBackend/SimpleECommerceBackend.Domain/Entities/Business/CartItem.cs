using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class CartItem : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private CartItem()
    {
    }

    private CartItem(Guid productId, Guid cartId, int quantity)
    {
        SetId(Guid.NewGuid());
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
            throw new ValidationException(
                CartItemErrorCodes.ProductIdRequired,
                "Product ID is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "ProductId"
                }
            );

        ProductId = productId;
    }

    private void SetCartId(Guid cartId)
    {
        if (cartId == Guid.Empty)
            throw new ValidationException(
                CartItemErrorCodes.CartIdRequired,
                "Cart ID is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "CartId"
                }
            );

        CartId = cartId;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException(
                CartItemErrorCodes.QuantityMustBeGreaterThanZero,
                "Quantity must be greater than zero",
                new Dictionary<string, object?>
                {
                    ["field"] = "Quantity"
                }
            );

        if (quantity > CartValidationRules.MaxQuantityPerItem)
            throw new ValidationException(
                CartItemErrorCodes.QuantityCannotExceed,
                $"Quantity cannot exceed {CartValidationRules.MaxQuantityPerItem}",
                new Dictionary<string, object?>
                {
                    ["field"] = "Quantity",
                    ["max"] = CartValidationRules.MaxQuantityPerItem
                }
            );

        Quantity = quantity;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ValidationException(
                CartItemErrorCodes.AmountMustBePositive,
                "Amount must be positive",
                new Dictionary<string, object?>
                {
                    ["field"] = "Amount"
                }
            );

        SetQuantity(Quantity + amount);
    }

    public void DecreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new ValidationException(
                CartItemErrorCodes.AmountMustBePositive,
                "Amount must be positive",
                new Dictionary<string, object?>
                {
                    ["field"] = "Amount"
                }
            );

        SetQuantity(Quantity - amount);
    }
}