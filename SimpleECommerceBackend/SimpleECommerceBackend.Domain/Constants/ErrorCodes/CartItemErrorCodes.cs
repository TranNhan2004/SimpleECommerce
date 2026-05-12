namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class CartItemErrorCodes
{
    public const string ProductVariantIdRequired = "CartItem_ProductVariantIdRequired";
    public const string CartIdRequired = "CartItem_CartIdRequired";
    public const string QuantityMustBeGreaterThanZero = "CartItem_QuantityMustBeGreaterThanZero";
    public const string QuantityCannotExceed = "CartItem_QuantityCannotExceed";
    public const string AmountMustBePositive = "CartItem_AmountMustBePositive";
}
