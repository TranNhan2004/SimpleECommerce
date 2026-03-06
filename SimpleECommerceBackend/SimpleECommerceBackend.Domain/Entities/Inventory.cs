using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Entities.Abstracts;

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
            throw new BusinessException("Product is required");

        ProductId = productId;
    }

    private void SetSellerWarehouseId(Guid sellerWarehouseId)
    {
        if (sellerWarehouseId == Guid.Empty)
            throw new BusinessException("Seller warehouse is required");

        SellerWarehouseId = sellerWarehouseId;
    }


    public void SetQuantityOnHand(int quantityOnHand)
    {
        if (quantityOnHand < 0)
            throw new BusinessException("Quantity on hand cannot be negative");

        if (quantityOnHand > InventoryConstants.MaxQuantity)
            throw new BusinessException($"Quantity on hand cannot exceed {InventoryConstants.MaxQuantity}");

        QuantityInStock = quantityOnHand;
    }

    public void SetQuantityReserved(int quantityReserved)
    {
        if (quantityReserved < 0)
            throw new BusinessException("Quantity reserved cannot be negative");

        if (quantityReserved > QuantityInStock)
            throw new BusinessException("Quantity reserved cannot exceed quantity on hand");

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
            throw new BusinessException("Quantity to add must be positive");

        SetQuantityOnHand(QuantityInStock + quantity);
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessException("Quantity to reserve must be positive");

        if (AvailableQuantity < quantity)
            throw new BusinessException("Insufficient available stock");

        SetQuantityReserved(QuantityReserved + quantity);
    }

    public void ReleaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessException("Quantity to release must be positive");

        if (QuantityReserved < quantity)
            throw new BusinessException("Cannot release more than reserved quantity");

        SetQuantityReserved(QuantityReserved - quantity);
    }
}