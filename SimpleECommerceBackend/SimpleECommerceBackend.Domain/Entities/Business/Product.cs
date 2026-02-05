using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.Interfaces.Time;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Product : EntityBase, ICreatedTime, IUpdatedTime, ISoftDeletable
{
    private Product()
    {
    }

    private Product(
        string name,
        string description,
        Money currentPrice,
        ProductStatusEnum status,
        Guid categoryId,
        Guid sellerId
    )
    {
        SetName(name);
        SetDescription(description);
        SetCurrentPrice(currentPrice);
        SetStatus(status);
        SetCategoryId(categoryId);
        SetSellerId(sellerId);
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money CurrentPrice { get; private set; } = new(0, "VND");
    public ProductStatusEnum Status { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    public Guid SellerId { get; private set; }
    public UserProfile? Seller { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public void SoftDelete(IClock clock)
    {
        if (IsDeleted)
            throw new DomainException("Product is deleted");

        IsDeleted = true;
        DeletedAt = clock.UtcNow;
    }

    public DateTimeOffset? UpdatedAt { get; private set; }


    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required");

        var trimmedName = name.Trim();
        if (trimmedName.Length > ProductConstants.NameMaxLength)
            throw new DomainException($"Name cannot exceed {ProductConstants.NameMaxLength} characters");

        Name = trimmedName;
    }

    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description is required");

        var trimmedDescription = description.Trim();
        if (trimmedDescription.Length > ProductConstants.DescriptionMaxLength)
            throw new DomainException(
                $"Description cannot exceed {ProductConstants.DescriptionMaxLength} characters");

        Description = trimmedDescription;
    }

    public void SetCurrentPrice(Money currentPrice)
    {
        CurrentPrice = currentPrice;
    }

    public void SetStatus(ProductStatusEnum status)
    {
        Status = status;
    }

    public void SetCategoryId(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainException("Category is required");

        CategoryId = categoryId;
    }

    public void SetSellerId(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
            throw new DomainException("Seller is required");

        SellerId = sellerId;
    }

    public static Product Create(
        string name,
        string description,
        Money currentPrice,
        ProductStatusEnum status,
        Guid categoryId,
        Guid sellerId
    )
    {
        return new Product(name, description, currentPrice, status, categoryId, sellerId);
    }
}