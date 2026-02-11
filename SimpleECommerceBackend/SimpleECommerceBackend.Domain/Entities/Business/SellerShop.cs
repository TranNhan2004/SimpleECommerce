using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class SellerShop : EntityBase, ICreatedTime, IUpdatedTime
{
    private List<SellerWarehouse> _sellerWarehouses = [];
    
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
        SetSellerId(sellerId);
        SetName(name);
        SetPhoneNumber(phoneNumber);
        SetAvatarUrl(avatarUrl);
    }

    public Guid SellerId { get; private set; }
    public UserProfile? Seller { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    
    public IReadOnlyCollection<SellerWarehouse> SellerWarehouses => _sellerWarehouses;
    
    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Seller shop name is required");

        var trimmedName = name.Trim();

        if (trimmedName.Length > SellerShopConstants.NameMaxLength)
            throw new DomainException(
                $"Seller shop name cannot exceed {SellerShopConstants.NameMaxLength} characters");

        Name = trimmedName;
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number is required");

        var trimmedPhoneNumber = phoneNumber.Trim();

        if (trimmedPhoneNumber.Length > CommonConstants.PhoneNumberMaxLength)
            throw new DomainException(
                $"Phone number cannot exceed {CommonConstants.PhoneNumberMaxLength} characters");

        PhoneNumber = trimmedPhoneNumber;
    }

    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }

    public void SetSellerId(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
            throw new DomainException("Seller is required");

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
            throw new DomainException("Warehouse not found");
        
        existing.SetFullAddress(sellerWarehouse.FullAddress);
    }

    public void RemoveSellerWarehouse(Guid id)
    {
        var existing = _sellerWarehouses.FirstOrDefault(s => s.Id == id);
        if (existing is null)
            throw new DomainException("Warehouse not found");
        
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