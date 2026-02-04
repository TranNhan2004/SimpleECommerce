using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class UserProfile : EntityBase, IAuditable, ISoftDeletable
{
    private UserProfile()
    {
    }

    private UserProfile(
        Guid credentialId,
        string email,
        string firstName,
        string lastName,
        DateOnly birthDate
    )
    {
        SetCredentialId(credentialId);
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetBirthDate(birthDate);
    }


    public Guid CredentialId { get; private set; }
    public Credential? Credential { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public void SoftDelete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }


    public void SetCredentialId(Guid credentialId)
    {
        if (credentialId == Guid.Empty)
            throw new DomainException("Credential ID is required");

        CredentialId = credentialId;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new DomainException("User email is required");

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > CredentialConstants.EmailMaxLength)
            throw new DomainException($"User email cannot exceed {CredentialConstants.EmailMaxLength} characters");

        if (!Regex.IsMatch(trimmedEmail, CredentialConstants.EmailPattern))
            throw new DomainException("User email is invalid");

        Email = trimmedEmail;
    }


    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            throw new DomainException("User firstname is required");

        var trimmedFirstName = firstName.Trim();

        if (trimmedFirstName.Length > UserProfileConstants.FirstNameMaxLength)
            throw new DomainException(
                $"User firstname cannot exceed {UserProfileConstants.FirstNameMaxLength} characters");

        FirstName = trimmedFirstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            throw new DomainException("User lastname is required");

        var trimmedLastName = lastName.Trim();

        if (trimmedLastName.Length > UserProfileConstants.LastNameMaxLength)
            throw new DomainException(
                $"User lastname cannot exceed {UserProfileConstants.LastNameMaxLength} characters");

        LastName = trimmedLastName;
    }

    public void SetBirthDate(DateOnly birthDate)
    {
        BirthDate = birthDate;
    }

    public static UserProfile Create(
        Guid credentialId,
        string email,
        string firstName,
        string lastName,
        DateOnly birthDate
    )
    {
        return new UserProfile(credentialId, email, firstName, lastName, birthDate);
    }
}