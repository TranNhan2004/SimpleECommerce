using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.RegExps;

namespace SimpleECommerceBackend.Domain.Utils;

public static class CredentialUtils
{
    public static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException(
                CredentialErrorCodes.EmailRequired,
                "Email is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Email"
                }
            );

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > CredentialValidationRules.EmailMaxLength)
            throw new ValidationException(
                CredentialErrorCodes.EmailMaxLengthExceeded,
                $"Email cannot exceed {CredentialValidationRules.EmailMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Email",
                    ["max"] = CredentialValidationRules.EmailMaxLength
                }
            );

        if (!CredentialRegex.EmailPattern().IsMatch(trimmedEmail))
            throw new ValidationException(
                CredentialErrorCodes.EmailInvalidFormat,
                "Email format is invalid",
                new Dictionary<string, object?>
                {
                    ["field"] = "Email"
                }
            );

        return trimmedEmail;
    }

    public static string ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ValidationException(
                CredentialErrorCodes.PasswordRequired,
                "Password is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Password"
                }
            );

        var trimmedPassword = password.Trim();

        if (trimmedPassword.Length < CredentialValidationRules.PasswordMinLength)
            throw new ValidationException(
                CredentialErrorCodes.PasswordMinLengthNotMet,
                $"Password must be at least {CredentialValidationRules.PasswordMinLength} characters long",
                new Dictionary<string, object?>
                {
                    ["field"] = "Password",
                    ["min"] = CredentialValidationRules.PasswordMinLength
                }
            );

        if (trimmedPassword.Length > CredentialValidationRules.PasswordMaxLength)
            throw new ValidationException(
                CredentialErrorCodes.PasswordMaxLengthExceeded,
                $"Password cannot exceed {CredentialValidationRules.PasswordMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Password",
                    ["max"] = CredentialValidationRules.PasswordMaxLength
                }
            );

        if (!CredentialRegex.PasswordPattern().IsMatch(trimmedPassword))
            throw new ValidationException(
                CredentialErrorCodes.PasswordInvalidFormat,
                "Password format is invalid",
                new Dictionary<string, object?>
                {
                    ["field"] = "Password"
                }
            );

        return trimmedPassword;
    }
}