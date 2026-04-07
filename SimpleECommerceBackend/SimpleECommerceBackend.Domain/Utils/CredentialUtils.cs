using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.RegExps;

namespace SimpleECommerceBackend.Domain.Utils;

public class CredentialUtils
{
    public static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException(
                CredentialErrorCode.EmailRequired,
                "Email is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Email"
                }
            );

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > CredentialConstants.EmailMaxLength)
            throw new ValidationException(
                CredentialErrorCode.EmailMaxLengthExceeded,
                $"Email cannot exceed {CredentialConstants.EmailMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Email",
                    ["max"] = CredentialConstants.EmailMaxLength
                }
            );

        if (!CredentialRegex.EmailPattern().IsMatch(trimmedEmail))
            throw new ValidationException(
                CredentialErrorCode.EmailInvalidFormat,
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
                CredentialErrorCode.PasswordRequired,
                "Password is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Password"
                }
            );

        var trimmedPassword = password.Trim();

        if (trimmedPassword.Length < CredentialConstants.PasswordMinLength)
            throw new ValidationException(
                CredentialErrorCode.PasswordMinLengthNotMet,
                $"Password must be at least {CredentialConstants.PasswordMinLength} characters long",
                new Dictionary<string, object?>
                {
                    ["field"] = "Password",
                    ["min"] = CredentialConstants.PasswordMinLength
                }
            );

        if (trimmedPassword.Length > CredentialConstants.PasswordMaxLength)
            throw new ValidationException(
                CredentialErrorCode.PasswordMaxLengthExceeded,
                $"Password cannot exceed {CredentialConstants.PasswordMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Password",
                    ["max"] = CredentialConstants.PasswordMaxLength
                }
            );

        if (!CredentialRegex.PasswordPattern().IsMatch(trimmedPassword))
            throw new ValidationException(
                CredentialErrorCode.PasswordInvalidFormat,
                "Password format is invalid",
                new Dictionary<string, object?>
                {
                    ["field"] = "Password"
                }
            );

        return trimmedPassword;
    }
}
