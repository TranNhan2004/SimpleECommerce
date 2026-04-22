namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class CustomerShippingAddressErrorCodes
{
    public const string AlreadyDeleted = "CustomerShippingAddress_AlreadyDeleted";
    public const string RecipientNameRequired = "CustomerShippingAddress_RecipientNameRequired";
    public const string RecipientNameMaxLengthExceeded = "CustomerShippingAddress_RecipientNameMaxLengthExceeded";
    public const string RecipientPhoneNumberRequired = "CustomerShippingAddress_RecipientPhoneNumberRequired";
    public const string RecipientPhoneNumberMaxLengthExceeded = "CustomerShippingAddress_RecipientPhoneNumberMaxLengthExceeded";
    public const string AlreadyDefault = "CustomerShippingAddress_AlreadyDefault";
    public const string NotDefault = "CustomerShippingAddress_NotDefault";
}
