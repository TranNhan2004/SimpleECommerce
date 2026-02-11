using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Product : EntityBase, ICreatedTime, IUpdatedTime
{
    private readonly List<ProductImage> _productImages = [];
    private readonly List<ProductPrice> _productPrices = [];

    private Product()
    {
    }

    private Product(
        string name,
        string description,
        Money currentPrice,
        ProductStatus status,
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
    public int TotalInStock { get; private set; }
    public ProductStatus Status { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    public Guid SellerId { get; private set; }
    public UserProfile? Seller { get; private set; }
    public IReadOnlyCollection<ProductImage> ProductImages => _productImages;
    public IReadOnlyCollection<ProductPrice> ProductPrices => _productPrices;


    public DateTimeOffset CreatedAt { get; private set; }

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

    public void SetStatus(ProductStatus status)
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

    public void AddImage(ProductImage image)
    {
        _productImages.Add(image);
    }

    public void ChangeImage(ProductImage image)
    {
        var existingImage = _productImages.FirstOrDefault(pi => pi.Id == image.Id);
        if (existingImage is null)
            throw new DomainException("Image not found");

        existingImage.SetDescription(image.Description);
        existingImage.SetDisplayOrder(image.DisplayOrder);
        existingImage.SetIsDisplayed(image.IsDisplayed);
    }

    public void RemoveImage(Guid imageId)
    {
        var existingImage = _productImages.FirstOrDefault(pi => pi.Id == imageId);
        if (existingImage is null)
            throw new DomainException("Image not found");

        _productImages.Remove(existingImage);
    }

    public void AddPrice(ProductPrice price)
    {
        _productPrices.Add(price);
    }


    public static Product Create(
        string name,
        string description,
        Money currentPrice,
        ProductStatus status,
        Guid categoryId,
        Guid sellerId
    )
    {
        return new Product(name, description, currentPrice, status, categoryId, sellerId);
    }
}