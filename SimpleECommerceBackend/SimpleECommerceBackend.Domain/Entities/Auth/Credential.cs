using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Auth;

public class Credential : EntityBase, IAuditable
{
    private Credential()
    {
    }

    private Credential(string email, string passwordHash)
    {
        SetEmail(email);
        SetPasswordHash(passwordHash);
    }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Guid RoleId { get; private set; }
    public Role? Role { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Credential Create(string email, string passwordHash)
    {
        return new Credential(email, passwordHash);
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new DomainException("Credential email is required");

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > CredentialConstants.EmailMaxLength)
            throw new DomainException(
                $"Credential email cannot exceed {CredentialConstants.EmailMaxLength} characters");

        if (!Regex.IsMatch(trimmedEmail, CredentialConstants.EmailPattern))
            throw new DomainException("Credential email is invalid");

        Email = trimmedEmail;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Credential password hash is required");

        if (passwordHash.Length < CredentialConstants.PasswordHashMinLength)
            throw new DomainException("Credential password hash is invalid");

        PasswordHash = passwordHash;
    }
}