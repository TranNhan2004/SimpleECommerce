namespace SimpleECommerceBackend.Domain.Constants.Auth;

public static class CredentialConstants
{
    public const int EmailMaxLength = 256;
    public const string EmailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
    public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d])[^\s]{8,}$";
}