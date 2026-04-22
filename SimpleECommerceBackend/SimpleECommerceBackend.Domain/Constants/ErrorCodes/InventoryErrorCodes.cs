namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class InventoryErrorCodes
{
    public const string ProductRequired = "Inventory_ProductRequired";
    public const string SellerWarehouseRequired = "Inventory_SellerWarehouseRequired";
    public const string QuantityOnHandCannotBeNegative = "Inventory_QuantityOnHandCannotBeNegative";
    public const string QuantityOnHandCannotExceed = "Inventory_QuantityOnHandCannotExceed";
    public const string QuantityReservedCannotBeNegative = "Inventory_QuantityReservedCannotBeNegative";
    public const string QuantityReservedCannotExceedQuantityOnHand = "Inventory_QuantityReservedCannotExceedQuantityOnHand";
    public const string QuantityToAddMustBePositive = "Inventory_QuantityToAddMustBePositive";
    public const string QuantityToReserveMustBePositive = "Inventory_QuantityToReserveMustBePositive";
    public const string InsufficientStock = "Inventory_InsufficientStock";
    public const string QuantityToReleaseMustBePositive = "Inventory_QuantityToReleaseMustBePositive";
    public const string QuantityToReleaseCannotExceedReserved = "Inventory_QuantityToReleaseCannotExceedReserved";
}
