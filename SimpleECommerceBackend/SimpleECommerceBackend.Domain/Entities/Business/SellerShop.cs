using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class SellerShop : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private readonly List<SellerWarehouse> _sellerWarehouses = [];

    private SellerShop()
    {
    }

    private SellerShop(
        Guid sellerId,
        string name,
        string phoneNumber,
        string? avatarUrl
    )
    {
        SetId(Guid.NewGuid());
        SetSellerId(sellerId);
        SetName(name);
        SetPhoneNumber(phoneNumber);
        SetAvatarUrl(avatarUrl);
    }

    public Guid SellerId { get; private set; }
    public UserProfile? Seller { get; private set; }

    public string Name { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string? AvatarUrl { get; private set; }

    public IReadOnlyCollection<SellerWarehouse> SellerWarehouses => _sellerWarehouses;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException(
                SellerShopErrorCode.NameRequired,
                "Seller shop name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "SellerShop"
                }
            );

        var trimmedName = name.Trim();

        if (trimmedName.Length > SellerShopConstants.NameMaxLength)
            throw new ValidationException(
                SellerShopErrorCode.NameMaxLengthExceeded,
                $"Seller shop name cannot exceed {SellerShopConstants.NameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "SellerShop",
                    ["max"] = SellerShopConstants.NameMaxLength
                }
            );

        Name = trimmedName;
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ValidationException(
                SellerShopErrorCode.PhoneNumberRequired,
                "Phone number is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "PhoneNumber"
                }
            );

        var trimmedPhoneNumber = phoneNumber.Trim();

        if (trimmedPhoneNumber.Length > CommonConstants.PhoneNumberMaxLength)
            throw new ValidationException(
                SellerShopErrorCode.PhoneNumberMaxLengthExceeded,
                $"Phone number cannot exceed {CommonConstants.PhoneNumberMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "PhoneNumber",
                    ["max"] = CommonConstants.PhoneNumberMaxLength
                }
            );

        PhoneNumber = trimmedPhoneNumber;
    }

    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }

    public void SetSellerId(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
            throw new ValidationException(
                SellerShopErrorCode.SellerRequired,
                "Seller is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Seller"
                }
            );

        SellerId = sellerId;
    }

    public void AddSellerWarehouse(SellerWarehouse sellerWarehouse)
    {
        _sellerWarehouses.Add(sellerWarehouse);
    }

    public void ChangeSellerWarehouse(SellerWarehouse sellerWarehouse)
    {
        var existing = _sellerWarehouses.FirstOrDefault(s => s.Id == sellerWarehouse.Id);
        if (existing is null)
            throw new ValidationException(
                SellerShopErrorCode.WarehouseNotFound,
                "Warehouse not found",
                new Dictionary<string, object?>
                {
                    ["field"] = "Warehouse"
                }
            );

        existing.SetFullAddress(sellerWarehouse.FullAddress);
    }

    public void RemoveSellerWarehouse(Guid id)
    {
        var existing = _sellerWarehouses.FirstOrDefault(s => s.Id == id);
        if (existing is null)
            throw new ValidationException(
                SellerShopErrorCode.WarehouseNotFound,
                "Warehouse not found",
                new Dictionary<string, object?>
                {
                    ["field"] = "Warehouse"
                }
            );

        existing.SoftDelete();
    }

    public static SellerShop Create(
        Guid sellerId,
        string name,
        string phoneNumber,
        string avatarUrl
    )
    {
        return new SellerShop(
            sellerId,
            name,
            phoneNumber,
            avatarUrl
        );
    }
}