using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Entities;

/// <summary>
/// Represents a user's business profile in the application.
/// The Id property stores the Keycloak user ID (sub claim) for authentication correlation.
/// </summary>
public class UserProfile : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private UserProfile()
    {
    }

    private UserProfile(
        Guid keycloakUserId,
        string email,
        string firstName,
        string lastName,
        string? nickName,
        Sex sex,
        DateOnly birthDate,
        string? avatarUrl
    )
    {
        SetId(keycloakUserId);
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetNickName(nickName);
        SetSex(sex);
        SetBirthDate(birthDate);
        SetAvatarUrl(avatarUrl);
    }

    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? NickName { get; private set; }
    public Sex Sex { get; private set; }
    public UserStatus Status { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new BusinessException("Email is required");

        var trimmedEmail = email.Trim();

        Email = trimmedEmail;
    }


    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            throw new BusinessException("First name is required");

        var trimmedFirstName = firstName.Trim();

        if (trimmedFirstName.Length > UserProfileConstants.FirstNameMaxLength)
            throw new BusinessException(
                $"First name cannot exceed {UserProfileConstants.FirstNameMaxLength} characters");

        FirstName = trimmedFirstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            throw new BusinessException("Last name is required");

        var trimmedLastName = lastName.Trim();

        if (trimmedLastName.Length > UserProfileConstants.LastNameMaxLength)
            throw new BusinessException(
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
            throw new BusinessException("Nick name is not blank");

        var trimmedNickName = nickName.Trim();

        if (trimmedNickName.Length > UserProfileConstants.NickNameMaxLength)
            throw new BusinessException($"Nick name cannot exceed {UserProfileConstants.NickNameMaxLength} characters");

        NickName = trimmedNickName;
    }

    public void SetSex(Sex sex)
    {
        Sex = sex;
    }

    public void SetBirthDate(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.UtcDateTime);

        if (birthDate > today)
            throw new BusinessException("Birth date cannot be in the future");

        if (AgeUtils.Calculate(birthDate, today) < UserProfileConstants.MinAge)
            throw new BusinessException($"Age cannot be less than {UserProfileConstants.MinAge} years");

        if (AgeUtils.Calculate(birthDate, today) > UserProfileConstants.MaxAge)
            throw new BusinessException($"Age cannot exceed than {UserProfileConstants.MaxAge} years");

        BirthDate = birthDate;
    }


    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }

    public static UserProfile Create(
        Guid keycloakUserId,
        string email,
        string firstName,
        string lastName,
        string? nickName,
        Sex sex,
        DateOnly birthDate,
        string? avatarUrl
    )
    {
        return new UserProfile(
            keycloakUserId,
            email,
            firstName,
            lastName,
            nickName,
            sex,
            birthDate,
            avatarUrl
        );
    }
}