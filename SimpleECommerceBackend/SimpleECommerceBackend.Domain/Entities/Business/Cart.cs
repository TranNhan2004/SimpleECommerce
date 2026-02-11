using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Cart : EntityBase, ICreatedTime, IUpdatedTime
{
    private Cart()
    {
    }

    private Cart(Guid customerId)
    {
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
            throw new DomainException("Customer ID is required");

        CustomerId = customerId;
    }

    public void AddCartItem(CartItem cartItem)
    {
        CartItems.Add(cartItem);
    }
}