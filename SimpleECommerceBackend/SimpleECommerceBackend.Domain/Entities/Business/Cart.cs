using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Cart : EntityBase
{
    public Cart()
    {
    }

    // private Cart(Guid customerId)
    // {
    //     Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
    //     CustomerId = customerId;
    // }

    private Guid _customerId;

    public Guid CustomerId
    {
        get => _customerId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    CartErrorCodes.CustomerIdRequired,
                    "Customer ID is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "CustomerId"
                    }
                );

            _customerId = value;
        }
    }

    public User? Customer { get; private set; }

    public List<CartItem> CartItems { get; } = [];

    public void AddCartItem(CartItem cartItem)
    {
        CartItems.Add(cartItem);
    }
}
