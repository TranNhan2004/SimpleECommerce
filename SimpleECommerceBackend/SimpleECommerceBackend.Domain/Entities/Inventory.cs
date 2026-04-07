using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities;

public class Inventory : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private Inventory()
    {
    }

    private Inventory(Guid productId, Guid sellerWarehouseId, int quantityOnHand, int quantityReserved)
    {
        SetId(Guid.NewGuid());
        SetProductId(productId);
        SetSellerWarehouseId(sellerWarehouseId);
        SetQuantityOnHand(quantityOnHand);
        SetQuantityReserved(quantityReserved);
        SetVersion(1);
    }

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }

    public Guid SellerWarehouseId { get; private set; }
    public SellerWarehouse? SellerWarehouse { get; private set; }

    public int QuantityInStock { get; private set; }
    public int QuantityReserved { get; private set; }
    public int Version { get; private set; }

    public int AvailableQuantity => QuantityInStock - QuantityReserved;

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Inventory Create(Guid productId, Guid sellerWarehouseId, int quantityOnHand)
    {
        return new Inventory(productId, sellerWarehouseId, quantityOnHand, 0);
    }

    private void SetProductId(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new ValidationException(
                InventoryErrorCode.ProductRequired,
                "Product is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Product"
                }
            );

        ProductId = productId;
    }

    private void SetSellerWarehouseId(Guid sellerWarehouseId)
    {
        if (sellerWarehouseId == Guid.Empty)
            throw new ValidationException(
                InventoryErrorCode.SellerWarehouseRequired,
                "Seller warehouse is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "SellerWarehouse"
                }
            );

        SellerWarehouseId = sellerWarehouseId;
    }


    public void SetQuantityOnHand(int quantityOnHand)
    {
        if (quantityOnHand < 0)
            throw new ValidationException(
                InventoryErrorCode.QuantityOnHandCannotBeNegative,
                "Quantity on hand cannot be negative",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityOnHand"
                }
            );

        if (quantityOnHand > InventoryConstants.MaxQuantity)
            throw new ValidationException(
                InventoryErrorCode.QuantityOnHandCannotExceed,
                $"Quantity on hand cannot exceed {InventoryConstants.MaxQuantity}",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityOnHand",
                    ["max"] = InventoryConstants.MaxQuantity
                }
            );

        QuantityInStock = quantityOnHand;
    }

    public void SetQuantityReserved(int quantityReserved)
    {
        if (quantityReserved < 0)
            throw new ValidationException(
                InventoryErrorCode.QuantityReservedCannotBeNegative,
                "Quantity reserved cannot be negative",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityReserved"
                }
            );

        if (quantityReserved > QuantityInStock)
            throw new ValidationException(
                InventoryErrorCode.QuantityReservedCannotExceedQuantityOnHand,
                "Quantity reserved cannot exceed quantity on hand",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityReserved",
                    ["max"] = "quantity on hand"
                }
            );

        QuantityReserved = quantityReserved;
    }

    private void SetVersion(int version)
    {
        Version = version;
    }

    public void IncreaseVersion()
    {
        ++Version;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException(
                InventoryErrorCode.QuantityToAddMustBePositive,
                "Quantity to add must be positive",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityToAdd"
                }
            );

        SetQuantityOnHand(QuantityInStock + quantity);
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException(
                InventoryErrorCode.QuantityToReserveMustBePositive,
                "Quantity to reserve must be positive",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityToReserve"
                }
            );

        if (AvailableQuantity < quantity)
            throw new ValidationException(
                InventoryErrorCode.InsufficientStock,
                "Insufficient available stock",
                new Dictionary<string, object?>
                {
                    ["field"] = "Quantity"
                }
            );

        SetQuantityReserved(QuantityReserved + quantity);
    }

    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ValidationException(
                InventoryErrorCode.QuantityToReleaseMustBePositive,
                "Quantity to release must be positive",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityToRelease"
                }
            );

        if (QuantityReserved < quantity)
            throw new ValidationException(
                InventoryErrorCode.QuantityToReleaseCannotExceedReserved,
                "Cannot release more than reserved quantity",
                new Dictionary<string, object?>
                {
                    ["field"] = "QuantityToRelease",
                    ["max"] = "reserved quantity"
                }
            );

        SetQuantityReserved(QuantityReserved - quantity);
    }
}
