namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class OrderItemErrorCodes
{
    public const string ProductVariantIdRequired = "OrderItem_ProductVariantIdRequired";
    public const string OrderIdRequired = "OrderItem_OrderIdRequired";
    public const string QuantityMustBeGreaterThanZero = "OrderItem_QuantityMustBeGreaterThanZero";
    public const string AmountCannotBeNegative = "OrderItem_AmountCannotBeNegative";
}
