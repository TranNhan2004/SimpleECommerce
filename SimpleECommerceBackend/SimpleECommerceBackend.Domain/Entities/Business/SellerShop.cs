using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class SellerShop : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private readonly List<SellerWarehouse> _sellerWarehouses = [];

    public SellerShop()
    {
    }

    private SellerShop(
        Guid sellerId,
        string name,
        string phoneNumber,
        string? avatarUrl
    )
    {
        Id = Guid.NewGuid();
        SellerId = sellerId;
        Name = name;
        PhoneNumber = phoneNumber;
        AvatarUrl = avatarUrl;
    }

    private Guid _sellerId;
    private string _name = null!;
    private string _phoneNumber = null!;
    private string? _avatarUrl;

    public Guid SellerId
    {
        get => _sellerId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    SellerShopErrorCodes.SellerRequired,
                    "Seller is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Seller"
                    }
                );

            _sellerId = value;
        }
    }

    public UserProfile? Seller { get; private set; }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    SellerShopErrorCodes.NameRequired,
                    "Seller shop name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "SellerShop"
                    }
                );

            var trimmedName = value.Trim();

            if (trimmedName.Length > SellerShopValidationRules.NameMaxLength)
                throw new ValidationException(
                    SellerShopErrorCodes.NameMaxLengthExceeded,
                    $"Seller shop name cannot exceed {SellerShopValidationRules.NameMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "SellerShop",
                        ["max"] = SellerShopValidationRules.NameMaxLength
                    }
                );

            _name = trimmedName;
        }
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    SellerShopErrorCodes.PhoneNumberRequired,
                    "Phone number is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "PhoneNumber"
                    }
                );

            var trimmedPhoneNumber = value.Trim();

            if (trimmedPhoneNumber.Length > CommonValidationRules.PhoneNumberMaxLength)
                throw new ValidationException(
                    SellerShopErrorCodes.PhoneNumberMaxLengthExceeded,
                    $"Phone number cannot exceed {CommonValidationRules.PhoneNumberMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "PhoneNumber",
                        ["max"] = CommonValidationRules.PhoneNumberMaxLength
                    }
                );

            _phoneNumber = trimmedPhoneNumber;
        }
    }

    public string? AvatarUrl
    {
        get => _avatarUrl;
        set => _avatarUrl = value;
    }

    public IReadOnlyCollection<SellerWarehouse> SellerWarehouses => _sellerWarehouses;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void AddSellerWarehouse(SellerWarehouse sellerWarehouse)
    {
        _sellerWarehouses.Add(sellerWarehouse);
    }

    public void ChangeSellerWarehouse(SellerWarehouse sellerWarehouse)
    {
        var existing = _sellerWarehouses.FirstOrDefault(s => s.Id == sellerWarehouse.Id);
        if (existing is null)
            throw new ValidationException(
                SellerShopErrorCodes.WarehouseNotFound,
                "Warehouse not found",
                new Dictionary<string, object?>
                {
                    ["field"] = "Warehouse"
                }
            );

        existing.FullAddress = sellerWarehouse.FullAddress;
    }

    public void RemoveSellerWarehouse(Guid id)
    {
        var existing = _sellerWarehouses.FirstOrDefault(s => s.Id == id);
        if (existing is null)
            throw new ValidationException(
                SellerShopErrorCodes.WarehouseNotFound,
                "Warehouse not found",
                new Dictionary<string, object?>
                {
                    ["field"] = "Warehouse"
                }
            );

        existing.SoftDelete();
    }

}
