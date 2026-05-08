using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class SellerWarehouse : Entity, ICreatedTrackable, IUpdatedTrackable, ISoftDeleteTrackable
{
    public SellerWarehouse()
    {
    }

    private SellerWarehouse(
        Address fullAddress,
        Guid sellerShopId
    )
    {
        Id = Guid.NewGuid();
        FullAddress = fullAddress;
        SellerShopId = sellerShopId;
    }

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

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public void SoftDelete()
    {
        if (IsDeleted)
            throw new ValidationException(
                SellerWarehouseErrorCodes.AlreadyDeleted,
                "Warehouse is already deleted",
                new Dictionary<string, object?>
                {
                    ["field"] = "Warehouse"
                }
            );

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset? UpdatedAt { get; private set; }

}
