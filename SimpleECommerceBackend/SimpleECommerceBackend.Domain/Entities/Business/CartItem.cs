using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class CartItem : Entity, ICreatedTrackable, IUpdatedTrackable
{
    public CartItem()
    {
    }

    private CartItem(Guid productId, Guid cartId, int quantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        CartId = cartId;
        Quantity = quantity;
    }

    private Guid _productId;
    private Guid _cartId;
    private int _quantity;

    public Guid ProductId
    {
        get => _productId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    CartItemErrorCodes.ProductIdRequired,
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

    public Guid CartId
    {
        get => _cartId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    CartItemErrorCodes.CartIdRequired,
                    "Cart ID is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "CartId"
                    }
                );

            _cartId = value;
        }
    }

    public Cart? Cart { get; private set; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
                throw new ValidationException(
                    CartItemErrorCodes.QuantityMustBeGreaterThanZero,
                    "Quantity must be greater than zero",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Quantity"
                    }
                );

            if (value > CartValidationRules.MaxQuantityPerItem)
                throw new ValidationException(
                    CartItemErrorCodes.QuantityCannotExceed,
                    $"Quantity cannot exceed {CartValidationRules.MaxQuantityPerItem}",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Quantity",
                        ["max"] = CartValidationRules.MaxQuantityPerItem
                    }
                );

            _quantity = value;
        }
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

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

        Quantity += amount;
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

        Quantity -= amount;
    }
}
