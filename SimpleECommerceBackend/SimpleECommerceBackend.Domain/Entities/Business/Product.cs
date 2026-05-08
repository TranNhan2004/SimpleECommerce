using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Product : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private readonly List<ProductImage> _productImages = [];
    private readonly List<ProductPrice> _productPrices = [];

    public Product()
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
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CurrentPrice = currentPrice;
        Status = status;
        CategoryId = categoryId;
        SellerId = sellerId;
    }

    private string _name = null!;
    private string _description = null!;
    private Money _currentPrice;
    private ProductStatus _status;
    private Guid _categoryId;
    private Guid _sellerId;

    private int _totalInStock;
    private double _averageRating;
    private int _totalRatings;
    private string? _defaultImageUrl;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ProductErrorCodes.NameRequired,
                    "Name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Name"
                    }
                );

            var trimmedName = value.Trim();
            if (trimmedName.Length > ProductValidationRules.NameMaxLength)
                throw new ValidationException(
                    ProductErrorCodes.NameMaxLengthExceeded,
                    $"Name cannot exceed {ProductValidationRules.NameMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Name",
                        ["max"] = ProductValidationRules.NameMaxLength
                    }
                );

            _name = trimmedName;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ProductErrorCodes.DescriptionRequired,
                    "Description is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description"
                    }
                );

            var trimmedDescription = value.Trim();
            if (trimmedDescription.Length > ProductValidationRules.DescriptionMaxLength)
                throw new ValidationException(
                    ProductErrorCodes.DescriptionMaxLengthExceeded,
                    $"Description cannot exceed {ProductValidationRules.DescriptionMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description",
                        ["max"] = ProductValidationRules.DescriptionMaxLength
                    }
                );

            _description = trimmedDescription;
        }
    }

    public Money CurrentPrice
    {
        get => _currentPrice;
        set => _currentPrice = value;
    }

    public int TotalInStock
    {
        get => _totalInStock;
        set
        {
            if (value < 0)
                throw new ValidationException(
                    ProductErrorCodes.TotalInStockCannotBeNegative,
                    "Total in stock cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "TotalInStock"
                    }
                );

            _totalInStock = value;
        }
    }
    public ProductStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public double AverageRating
    {
        get => _averageRating;
        set
        {
            if (value < 0 || value > 5)
                throw new ValidationException(
                    ProductErrorCodes.AverageRatingOutOfRange,
                    "Average rating must be between 0 and 5",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "AverageRating",
                        ["min"] = 0,
                        ["max"] = 5
                    }
                );

            _averageRating = value;
        }
    }

    public int TotalRatings
    {
        get => _totalRatings;
        set
        {
            if (value < 0)
                throw new ValidationException(
                    ProductErrorCodes.TotalRatingsCannotBeNegative,
                    "Total ratings cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "TotalRatings"
                    }
                );

            _totalRatings = value;
        }
    }

    public string? DefaultImageUrl
    {
        get => _defaultImageUrl;
        set => _defaultImageUrl = value;
    }

    public Guid CategoryId
    {
        get => _categoryId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ProductErrorCodes.CategoryRequired,
                    "Category is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Category"
                    }
                );

            _categoryId = value;
        }
    }

    public Category? Category { get; set; }

    public Guid SellerId
    {
        get => _sellerId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ProductErrorCodes.SellerRequired,
                    "Seller is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Seller"
                    }
                );

            _sellerId = value;
        }
    }

    public UserProfile? Seller { get; set; }
    public IReadOnlyCollection<ProductImage> ProductImages => _productImages;
    public IReadOnlyCollection<ProductPrice> ProductPrices => _productPrices;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void Activate()
    {
        if (Status != ProductStatus.Draft && Status != ProductStatus.Hidden)
            throw new ValidationException(
                ProductErrorCodes.ActivateNotAllowed,
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
                ProductErrorCodes.HideNotAllowed,
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

    public void AddImage(ProductImage image)
    {
        _productImages.Add(image);
    }

    public void ChangeImage(ProductImage image)
    {
        var existingImage = _productImages.FirstOrDefault(pi => pi.Id == image.Id);
        if (existingImage is null)
            throw new ValidationException(
                ProductErrorCodes.ImageNotFound,
                "Image not found",
                new Dictionary<string, object?>
                {
                    ["field"] = "Image"
                }
            );

        existingImage.Description = image.Description;
        existingImage.DisplayOrder = image.DisplayOrder;
        existingImage.IsDisplayed = image.IsDisplayed;
    }

    public void RemoveImage(Guid imageId)
    {
        var existingImage = _productImages.FirstOrDefault(pi => pi.Id == imageId);
        if (existingImage is null)
            throw new ValidationException(
                ProductErrorCodes.ImageNotFound,
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
}
