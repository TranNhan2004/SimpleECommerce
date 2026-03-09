using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.RegExps;

namespace SimpleECommerceBackend.Domain.Utils;

public class CredentialUtils
{
    public static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessException("Email is required");

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > CredentialConstants.EmailMaxLength)
            throw new BusinessException($"Email cannot exceed {CredentialConstants.EmailMaxLength} characters");

        if (!CredentialRegex.EmailPattern().IsMatch(trimmedEmail))
            throw new BusinessException("Email format is invalid");

        return trimmedEmail;
    }

    public static string ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new BusinessException("Password is required");

        var trimmedPassword = password.Trim();

        if (trimmedPassword.Length < CredentialConstants.PasswordMinLength)
            throw new BusinessException($"Password must be at least {CredentialConstants.PasswordMinLength} characters long");

        if (trimmedPassword.Length > CredentialConstants.PasswordMaxLength)
            throw new BusinessException($"Password cannot exceed {CredentialConstants.PasswordMaxLength} characters");

        if (!CredentialRegex.PasswordPattern().IsMatch(trimmedPassword))
            throw new BusinessException("Password format is invalid");

        return trimmedPassword;
    }
}