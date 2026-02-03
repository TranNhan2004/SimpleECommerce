namespace SimpleECommerceBackend.Domain.Constants;

public static class UserConstants
{
    public const int FirstNameMaxLength = 50;
    public const int LastNameMaxLength = 127;
    public const int EmailMaxLength = 255;
    public const string EmailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
    public const string PhoneNumberPattern = @"^0(3|5|7|8|9)\d+$";
    public const int PhoneNumberExactLength = 10;
}