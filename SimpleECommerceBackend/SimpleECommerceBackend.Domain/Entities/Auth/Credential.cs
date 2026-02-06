using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Auth;

public class Credential : EntityBase, ICreatedTime, IUpdatedTime
{
    private Credential()
    {
    }

    private Credential(string email, string passwordHash, Guid roleId)
    {
        SetEmail(email);
        SetPasswordHash(passwordHash);
        SetRoleId(roleId);
    }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public CredentialStatus Status { get; private set; }
    public Guid RoleId { get; private set; }
    public Role? Role { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Credential Create(string email, string passwordHash, Guid roleId)
    {
        return new Credential(email, passwordHash, roleId);
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new DomainException("Email is required");

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > CredentialConstants.EmailMaxLength)
            throw new DomainException(
                $"Email cannot exceed {CredentialConstants.EmailMaxLength} characters");

        if (!Regex.IsMatch(trimmedEmail, CredentialConstants.EmailPattern))
            throw new DomainException("Email is invalid");

        Email = trimmedEmail;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required");

        PasswordHash = passwordHash;
    }

    public void SetRoleId(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new DomainException("Role ID is required");

        RoleId = roleId;
    }
}