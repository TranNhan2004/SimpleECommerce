namespace SimpleECommerceBackend.Domain.Constants.ValidationRules;

public static class CommonValidationRules
{
    public const string PhoneNumberPattern = @"^0(3|5|7|8|9)\d+$";
    public const int PhoneNumberMaxLength = 10;
}
