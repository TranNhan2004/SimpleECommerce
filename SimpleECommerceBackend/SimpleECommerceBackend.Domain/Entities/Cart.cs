using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities;

public class Cart : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private Cart()
    {
    }

    private Cart(Guid customerId)
    {
        SetId(Guid.NewGuid());
        SetCustomerId(customerId);
    }

    public Guid CustomerId { get; private set; }
    public UserProfile? Customer { get; private set; }

    public List<CartItem> CartItems { get; } = [];

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Cart Create(Guid customerId)
    {
        return new Cart(customerId);
    }

    public void SetCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new BusinessException("Customer ID is required");

        CustomerId = customerId;
    }

    public void AddCartItem(CartItem cartItem)
    {
        CartItems.Add(cartItem);
    }
}