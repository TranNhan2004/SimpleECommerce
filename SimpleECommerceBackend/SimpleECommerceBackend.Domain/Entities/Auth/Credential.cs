using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Auth;

public class Credential : IEntity, ICreatedTrackable, IUpdatedTrackable
{
    private Credential()
    {
    }

    private Credential(string email, string passwordHash, CredentialStatus status, Role role)
    {
        SetEmail(email);
        SetPasswordHash(passwordHash);
        SetStatus(status);
        SetRole(role);
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public CredentialStatus Status { get; private set; }
    public Role Role { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Credential Create(string email, string passwordHash, Role role)
    {
        return new Credential(email, passwordHash, CredentialStatus.Inactive, role);
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new BusinessException("Email is required");

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > CredentialConstants.EmailMaxLength)
            throw new BusinessException($"Email cannot exceed {CredentialConstants.EmailMaxLength} characters");

        if (!Regex.IsMatch(trimmedEmail, CredentialConstants.EmailPattern))
            throw new BusinessException("Email is invalid");

        Email = trimmedEmail;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new BusinessException("Password hash is required");

        PasswordHash = passwordHash;
    }

    private void SetStatus(CredentialStatus status)
    {
        Status = status;
    }

    public void Activate()
    {
        if (Status != CredentialStatus.Inactive)
            throw new BusinessException("Only inactive credentials can be activated");

        Status = CredentialStatus.Active;
    }

    public void Deactivate()
    {
        if (Status != CredentialStatus.Active)
            throw new BusinessException("Only active credentials can be deactivated");

        Status = CredentialStatus.Inactive;
    }

    public void Archive()
    {
        if (Status != CredentialStatus.Inactive)
            throw new BusinessException("Only inactive credentials can be archived");

        Status = CredentialStatus.Archived;
    }

    public void SetRole(Role role)
    {
        Role = role;
    }
}