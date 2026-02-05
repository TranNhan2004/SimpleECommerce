using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.Interfaces.Time;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class UserProfile : EntityBase, ICreatedTime, IUpdatedTime, ISoftDeletable
{
    private UserProfile()
    {
    }

    private UserProfile(
        Guid credentialId,
        string email,
        string firstName,
        string lastName,
        string? nickName,
        SexEnum sex,
        DateOnly birthDate,
        string? avatarUrl,
        IClock clock
    )
    {
        SetCredentialId(credentialId);
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetNickName(nickName);
        SetSex(sex);
        SetBirthDate(birthDate, clock);
        SetAvatarUrl(avatarUrl);
    }


    public Guid CredentialId { get; private set; }
    public Credential? Credential { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? NickName { get; private set; }
    public SexEnum Sex { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string? AvatarUrl { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public void SoftDelete(IClock clock)
    {
        if (IsDeleted)
            throw new DomainException("User profile is deleted");

        IsDeleted = true;
        DeletedAt = clock.UtcNow;
    }

    public DateTimeOffset? UpdatedAt { get; private set; }


    public void SetCredentialId(Guid credentialId)
    {
        if (credentialId == Guid.Empty)
            throw new DomainException("Credential is required");

        CredentialId = credentialId;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new DomainException("Email is required");

        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length > CredentialConstants.EmailMaxLength)
            throw new DomainException($"Email cannot exceed {CredentialConstants.EmailMaxLength} characters");

        if (!Regex.IsMatch(trimmedEmail, CredentialConstants.EmailPattern))
            throw new DomainException("Email is invalid");

        Email = trimmedEmail;
    }


    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            throw new DomainException("First name is required");

        var trimmedFirstName = firstName.Trim();

        if (trimmedFirstName.Length > UserProfileConstants.FirstNameMaxLength)
            throw new DomainException(
                $"First name cannot exceed {UserProfileConstants.FirstNameMaxLength} characters");

        FirstName = trimmedFirstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            throw new DomainException("Last name is required");

        var trimmedLastName = lastName.Trim();

        if (trimmedLastName.Length > UserProfileConstants.LastNameMaxLength)
            throw new DomainException(
                $"Last name cannot exceed {UserProfileConstants.LastNameMaxLength} characters");

        LastName = trimmedLastName;
    }

    public void SetNickName(string? nickName)
    {
        if (nickName is null)
        {
            NickName = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(nickName))
            throw new DomainException("Nick name is not blank");

        var trimmedNickName = nickName.Trim();

        if (trimmedNickName.Length > UserProfileConstants.NickNameMaxLength)
            throw new DomainException($"Nick name cannot exceed {UserProfileConstants.NickNameMaxLength} characters");

        NickName = trimmedNickName;
    }

    public void SetSex(SexEnum sex)
    {
        Sex = sex;
    }

    public void SetBirthDate(DateOnly birthDate, IClock clock)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow.UtcDateTime);

        if (birthDate > today)
            throw new DomainException("Birth date cannot be in the future");

        if (AgeCalculator.Calculate(birthDate, today) < UserProfileConstants.MinAge)
            throw new DomainException($"Age cannot be less than {UserProfileConstants.MinAge} years");

        BirthDate = birthDate;
    }


    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }

    public static UserProfile Create(
        Guid credentialId,
        string email,
        string firstName,
        string lastName,
        string? nickName,
        SexEnum sex,
        DateOnly birthDate,
        string? avatarUrl,
        IClock clock
    )
    {
        return new UserProfile(credentialId, email, firstName, lastName, nickName, sex, birthDate, avatarUrl, clock);
    }
}