namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class CartItemErrorCode
{
    public const string ProductIdRequired = "CartItem_ProductIdRequired";
    public const string CartIdRequired = "CartItem_CartIdRequired";
    public const string QuantityMustBeGreaterThanZero = "CartItem_QuantityMustBeGreaterThanZero";
    public const string QuantityCannotExceed = "CartItem_QuantityCannotExceed";
    public const string AmountMustBePositive = "CartItem_AmountMustBePositive";
}
