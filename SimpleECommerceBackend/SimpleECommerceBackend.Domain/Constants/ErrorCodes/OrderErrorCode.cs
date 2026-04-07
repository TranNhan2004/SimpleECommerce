namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class OrderErrorCode
{
    public const string CodeRequired = "Order_CodeRequired";
    public const string CodeMaxLengthExceeded = "Order_CodeMaxLengthExceeded";
    public const string NoteMustNotBeBlank = "Order_NoteMustNotBeBlank";
    public const string NoteMaxLengthExceeded = "Order_NoteMaxLengthExceeded";
    public const string PickupNotAllowed = "Order_PickupNotAllowed";
    public const string ShipNotAllowed = "Order_ShipNotAllowed";
    public const string AwaitConfirmationNotAllowed = "Order_AwaitConfirmationNotAllowed";
    public const string DeliverNotAllowed = "Order_DeliverNotAllowed";
    public const string CancelNotAllowed = "Order_CancelNotAllowed";
    public const string ReturnNotAllowed = "Order_ReturnNotAllowed";
    public const string ExpireNotAllowed = "Order_ExpireNotAllowed";
    public const string ShopNameRequired = "Order_ShopNameRequired";
    public const string ShopNameMaxLengthExceeded = "Order_ShopNameMaxLengthExceeded";
    public const string RecipientNameRequired = "Order_RecipientNameRequired";
    public const string RecipientNameMaxLengthExceeded = "Order_RecipientNameMaxLengthExceeded";
    public const string RecipientPhoneNumberRequired = "Order_RecipientPhoneNumberRequired";
    public const string RecipientPhoneNumberMaxLengthExceeded = "Order_RecipientPhoneNumberMaxLengthExceeded";
    public const string CustomerRequired = "Order_CustomerRequired";
    public const string SellerRequired = "Order_SellerRequired";
}
