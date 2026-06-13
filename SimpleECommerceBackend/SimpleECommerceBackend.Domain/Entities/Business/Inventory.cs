using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Inventory : EntityBase
{
    public Inventory()
    {
    }

    // private Inventory(Guid productId, Guid sellerWarehouseId, int quantityOnHand, int quantityReserved)
    // {
    //     Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
    //     ProductId = productId;
    //     SellerWarehouseId = sellerWarehouseId;
    //     QuantityInStock = quantityOnHand;
    //     QuantityReserved = quantityReserved;
    //     Version = 1;
    // }

    private Guid _productVariantId;
    private Guid _sellerWarehouseId;
    private int _quantityInStock;
    private int _quantityReserved;
    private int _version;

    public Guid ProductVariantId
    {
        get => _productVariantId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    InventoryErrorCodes.ProductVariantRequired,
                    "Product variant is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ProductVariant"
                    }
                );

            _productVariantId = value;
        }
    }

    public ProductVariant? ProductVariant { get; private set; }

    public Guid SellerWarehouseId
    {
        get => _sellerWarehouseId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    InventoryErrorCodes.SellerWarehouseRequired,
                    "Seller warehouse is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "SellerWarehouse"
                    }
                );

            _sellerWarehouseId = value;
        }
    }

    public SellerWarehouse? SellerWarehouse { get; private set; }

    public int QuantityInStock
    {
        get => _quantityInStock;
        set
        {
            if (value < 0)
                throw new ValidationException(
                    InventoryErrorCodes.QuantityOnHandCannotBeNegative,
                    "Quantity on hand cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "QuantityOnHand"
                    }
                );

            if (value > InventoryValidationRules.MaxQuantity)
                throw new ValidationException(
                    InventoryErrorCodes.QuantityOnHandCannotExceed,
                    $"Quantity on hand cannot exceed {InventoryValidationRules.MaxQuantity}",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "QuantityOnHand",
                        ["max"] = InventoryValidationRules.MaxQuantity
                    }
                );

            _quantityInStock = value;
        }
    }

    public int QuantityReserved
    {
        get => _quantityReserved;
        set
        {
            if (value < 0)
                throw new ValidationException(
                    InventoryErrorCodes.QuantityReservedCannotBeNegative,
                    "Quantity reserved cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "QuantityReserved"
                    }
                );

            if (value > QuantityInStock)
                throw new ValidationException(
                    InventoryErrorCodes.QuantityReservedCannotExceedQuantityOnHand,
                    "Quantity reserved cannot exceed quantity on hand",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "QuantityReserved",
                        ["max"] = "quantity on hand"
                    }
                );

            _quantityReserved = value;
        }
    }

    public int Version
    {
        get => _version;
        set => _version = value;
    }

    public int AvailableQuantity => QuantityInStock - QuantityReserved;
    public void IncreaseVersion()
    {
        ++Version;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException(
                InventoryErrorCodes.QuantityToAddMustBePositive,
                "Quantity to add must be positive",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityToAdd"
                }
            );

        QuantityInStock += quantity;
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException(
                InventoryErrorCodes.QuantityToReserveMustBePositive,
                "Quantity to reserve must be positive",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityToReserve"
                }
            );

        if (AvailableQuantity < quantity)
            throw new ValidationException(
                InventoryErrorCodes.InsufficientStock,
                "Insufficient available stock",
                new Dictionary<string, object?>
                {
                    ["field"] = "Quantity"
                }
            );

        QuantityReserved += quantity;
    }

    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException(
                InventoryErrorCodes.QuantityToReleaseMustBePositive,
                "Quantity to release must be positive",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityToRelease"
                }
            );

        if (QuantityReserved < quantity)
            throw new ValidationException(
                InventoryErrorCodes.QuantityToReleaseCannotExceedReserved,
                "Cannot release more than reserved quantity",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityToRelease",
                    ["max"] = "reserved quantity"
                }
            );

        QuantityReserved -= quantity;
    }
}
