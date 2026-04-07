namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class CredentialErrorCode
{
    public const string EmailRequired = "Credential_EmailRequired";
    public const string EmailMaxLengthExceeded = "Credential_EmailMaxLengthExceeded";
    public const string EmailInvalidFormat = "Credential_EmailInvalidFormat";
    public const string PasswordRequired = "Credential_PasswordRequired";
    public const string PasswordMinLengthNotMet = "Credential_PasswordMinLengthNotMet";
    public const string PasswordMaxLengthExceeded = "Credential_PasswordMaxLengthExceeded";
    public const string PasswordInvalidFormat = "Credential_PasswordInvalidFormat";
}
