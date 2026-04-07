using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities;

public class Product : Entity, ICreatedTrackable, IUpdatedTrackable
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
        SetId(Guid.NewGuid());
        SetName(name);
        SetDescription(description);
        SetCurrentPrice(currentPrice);
        SetStatus(status);
        SetCategoryId(categoryId);
        SetSellerId(sellerId);
    }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Money CurrentPrice { get; private set; }
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
            throw new ValidationException(
                ProductErrorCode.NameRequired,
                "Name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Name"
                }
            );

        var trimmedName = name.Trim();
        if (trimmedName.Length > ProductConstants.NameMaxLength)
            throw new ValidationException(
                ProductErrorCode.NameMaxLengthExceeded,
                $"Name cannot exceed {ProductConstants.NameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Name",
                    ["max"] = ProductConstants.NameMaxLength
                }
            );

        Name = trimmedName;
    }

    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ValidationException(
                ProductErrorCode.DescriptionRequired,
                "Description is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Description"
                }
            );

        var trimmedDescription = description.Trim();
        if (trimmedDescription.Length > ProductConstants.DescriptionMaxLength)
            throw new ValidationException(
                ProductErrorCode.DescriptionMaxLengthExceeded,
                $"Description cannot exceed {ProductConstants.DescriptionMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Description",
                    ["max"] = ProductConstants.DescriptionMaxLength
                }
            );

        Description = trimmedDescription;
    }

    public void SetCurrentPrice(Money currentPrice)
    {
        CurrentPrice = currentPrice;
    }

    private void SetStatus(ProductStatus status)
    {
        Status = status;
    }

    public void Activate()
    {
        if (Status != ProductStatus.Draft && Status != ProductStatus.Hidden)
            throw new ValidationException(
                ProductErrorCode.ActivateNotAllowed,
                "Only draft or hidden product can be activated",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Activate",
                    ["allowedStates"] = "draft, hidden"
                }
            );

        Status = ProductStatus.Active;
    }

    public void Hide()
    {
        if (Status != ProductStatus.Draft && Status != ProductStatus.Active)
            throw new ValidationException(
                ProductErrorCode.HideNotAllowed,
                "Only draft or active product can be hidden",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Hide",
                    ["allowedStates"] = "draft, active"
                }
            );

        Status = ProductStatus.Hidden;
    }

    public void SetCategoryId(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ValidationException(
                ProductErrorCode.CategoryRequired,
                "Category is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Category"
                }
            );

        CategoryId = categoryId;
    }

    public void SetSellerId(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
            throw new ValidationException(
                ProductErrorCode.SellerRequired,
                "Seller is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Seller"
                }
            );

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
            throw new ValidationException(
                ProductErrorCode.ImageNotFound,
                "Image not found",
                new Dictionary<string, object?>
                {
                    ["field"] = "Image"
                }
            );

        existingImage.SetDescription(image.Description);
        existingImage.SetDisplayOrder(image.DisplayOrder);
        existingImage.SetIsDisplayed(image.IsDisplayed);
    }

    public void RemoveImage(Guid imageId)
    {
        var existingImage = _productImages.FirstOrDefault(pi => pi.Id == imageId);
        if (existingImage is null)
            throw new ValidationException(
                ProductErrorCode.ImageNotFound,
                "Image not found",
                new Dictionary<string, object?>
                {
                    ["field"] = "Image"
                }
            );

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
        Guid categoryId,
        Guid sellerId
    )
    {
        return new Product(name, description, currentPrice, ProductStatus.Draft, categoryId, sellerId);
    }
}
