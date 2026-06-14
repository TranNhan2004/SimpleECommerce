using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductVariant : EntityBase
{
    private readonly List<ProductVariantImage> _productVariantImages = [];
    private readonly List<ProductVariantPrice> _productVariantPrices = [];

    public ProductVariant()
    {
    }

    private Guid _productId;
    private string _name = null!;
    private string _description = null!;
    private Money _currentPrice;
    private int _totalInStock;

    public Guid ProductId
    {
        get => _productId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ProductVariantErrorCodes.ProductRequired,
                    "Product is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ProductId"
                    }
                );

            _productId = value;
        }
    }

    public Product? Product { get; private set; }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ProductVariantErrorCodes.NameRequired,
                    "Name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Name"
                    }
                );

            var trimmedName = value.Trim();
            if (trimmedName.Length > ProductVariantValidationRules.NameMaxLength)
                throw new ValidationException(
                    ProductVariantErrorCodes.NameMaxLengthExceeded,
                    $"Name cannot exceed {ProductVariantValidationRules.NameMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Name",
                        ["max"] = ProductVariantValidationRules.NameMaxLength
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
                    ProductVariantErrorCodes.DescriptionRequired,
                    "Description is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description"
                    }
                );

            var trimmedDescription = value.Trim();
            if (trimmedDescription.Length > ProductVariantValidationRules.DescriptionMaxLength)
                throw new ValidationException(
                    ProductVariantErrorCodes.DescriptionMaxLengthExceeded,
                    $"Description cannot exceed {ProductVariantValidationRules.DescriptionMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description",
                        ["max"] = ProductVariantValidationRules.DescriptionMaxLength
                    }
                );

            _description = trimmedDescription;
        }
    }

    public Money CurrentPrice
    {
        get => _currentPrice;
        set
        {
            if (value.Amount <= 0)
                throw new ValidationException(
                    ProductVariantErrorCodes.CurrentPriceMustBePositive,
                    "Current price must be positive",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "CurrentPrice"
                    }
                );

            _currentPrice = value;
        }
    }

    public int TotalInStock
    {
        get => _totalInStock;
        set
        {
            if (value < 0)
                throw new ValidationException(
                    ProductVariantErrorCodes.TotalInStockCannotBeNegative,
                    "Total in stock cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "TotalInStock"
                    }
                );

            _totalInStock = value;
        }
    }

    public string? DefaultImageUrl { get; set; }

    public ProductInvariantStatus Status { get; set; }

    public IReadOnlyCollection<ProductVariantImage> ProductVariantImages => _productVariantImages;
    public IReadOnlyCollection<ProductVariantPrice> ProductVariantPrices => _productVariantPrices;

    public void AddImage(ProductVariantImage image)
    {
        _productVariantImages.Add(image);
    }

    public void ChangeImage(ProductVariantImage image)
    {
        var existingImage = _productVariantImages.FirstOrDefault(pi => pi.Id == image.Id);
        if (existingImage is null)
            throw new ValidationException(
                ProductVariantErrorCodes.ImageNotFound,
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
        var existingImage = _productVariantImages.FirstOrDefault(pi => pi.Id == imageId);
        if (existingImage is null)
            throw new ValidationException(
                ProductVariantErrorCodes.ImageNotFound,
                "Image not found",
                new Dictionary<string, object?>
                {
                    ["field"] = "Image"
                }
            );

        _productVariantImages.Remove(existingImage);
    }

    public void AddPrice(ProductVariantPrice price)
    {
        _productVariantPrices.Add(price);
    }
}
