namespace SimpleECommerceBackend.Domain.Constants;

public static class UserProfileConstants
{
    public const int FirstNameMaxLength = 50;
    public const int LastNameMaxLength = 127;
    public const string PhoneNumberPattern = @"^0(3|5|7|8|9)\d+$";
    public const int PhoneNumberExactLength = 10;
}