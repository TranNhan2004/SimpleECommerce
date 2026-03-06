using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities;

public class SellerWarehouse : Entity, ICreatedTrackable, IUpdatedTrackable, ISoftDeleteTrackable
{
    private SellerWarehouse()
    {
    }

    private SellerWarehouse(
        Address fullAddress,
        Guid sellerShopId
    )
    {
        SetId(Guid.NewGuid());
        SetFullAddress(fullAddress);
        SetSellerShopId(sellerShopId);
    }

    public Address FullAddress { get; private set; }

    public Guid SellerShopId { get; private set; }
    public SellerShop? SellerShop { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public void SoftDelete()
    {
        if (IsDeleted)
            throw new BusinessException("Warehouse is already deleted");

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public static SellerWarehouse Create(
        Address fullAddress,
        Guid sellerShopId
    )
    {
        return new SellerWarehouse(
            fullAddress,
            sellerShopId
        );
    }

    public void SetFullAddress(Address fullAddress)
    {
        FullAddress = fullAddress;
    }

    private void SetSellerShopId(Guid sellerShopId)
    {
        if (sellerShopId == Guid.Empty)
            throw new BusinessException("Seller shop is required");

        SellerShopId = sellerShopId;
    }
}