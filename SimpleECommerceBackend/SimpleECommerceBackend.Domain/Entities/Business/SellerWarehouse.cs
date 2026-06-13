using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class SellerWarehouse : EntityBase
{
    public SellerWarehouse()
    {
    }

    // private SellerWarehouse(
    //     Address fullAddress,
    //     Guid sellerShopId
    // )
    // {
    //     Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
    //     FullAddress = fullAddress;
    //     SellerShopId = sellerShopId;
    // }

    private Address _fullAddress;
    private Guid _sellerShopId;

    public Address FullAddress
    {
        get => _fullAddress;
        set => _fullAddress = value;
    }

    public Guid SellerShopId
    {
        get => _sellerShopId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    SellerWarehouseErrorCodes.SellerShopRequired,
                    "Seller shop is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "SellerShop"
                    }
                );

            _sellerShopId = value;
        }
    }

    public SellerShop? SellerShop { get; private set; }
}
