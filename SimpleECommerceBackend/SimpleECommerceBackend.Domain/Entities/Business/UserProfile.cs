using System.Text.Json.Serialization;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Entities.Business;

/// <summary>
///     Represents a user's business profile in the application.
///     The Id property stores the Keycloak user ID (sub claim) for authentication correlation.
/// </summary>
public class UserProfile : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private UserProfile()
    {
    }

    [JsonConstructor]
    private UserProfile(
        Guid id,
        string email,
        string firstName,
        string lastName,
        string? nickName,
        Sex sex,
        UserStatus status,
        DateOnly birthDate,
        string? avatarUrl,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt
    )
    {
        SetId(id);
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetNickName(nickName);
        SetSex(sex);
        SetStatus(status);
        SetBirthDate(birthDate);
        SetAvatarUrl(avatarUrl);
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
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
        SetStatus(UserStatus.Active);
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
        Email = CredentialUtils.ValidateEmail(email);
    }

    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ValidationException(
                UserProfileErrorCodes.FirstNameRequired,
                "First name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "FirstName"
                }
            );

        var trimmedFirstName = firstName.Trim();

        if (trimmedFirstName.Length > UserProfileValidationRules.FirstNameMaxLength)
            throw new ValidationException(
                UserProfileErrorCodes.FirstNameMaxLengthExceeded,
                $"First name cannot exceed {UserProfileValidationRules.FirstNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "FirstName",
                    ["max"] = UserProfileValidationRules.FirstNameMaxLength
                }
            );

        FirstName = trimmedFirstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ValidationException(
                UserProfileErrorCodes.LastNameRequired,
                "Last name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "LastName"
                }
            );

        var trimmedLastName = lastName.Trim();

        if (trimmedLastName.Length > UserProfileValidationRules.LastNameMaxLength)
            throw new ValidationException(
                UserProfileErrorCodes.LastNameMaxLengthExceeded,
                $"Last name cannot exceed {UserProfileValidationRules.LastNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "LastName",
                    ["max"] = UserProfileValidationRules.LastNameMaxLength
                }
            );

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
            throw new ValidationException(
                UserProfileErrorCodes.NickNameMustNotBeBlank,
                "Nick name is not blank",
                new Dictionary<string, object?>
                {
                    ["field"] = "NickName"
                }
            );

        var trimmedNickName = nickName.Trim();

        if (trimmedNickName.Length > UserProfileValidationRules.NickNameMaxLength)
            throw new ValidationException(
                UserProfileErrorCodes.NickNameMaxLengthExceeded,
                $"Nick name cannot exceed {UserProfileValidationRules.NickNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "NickName",
                    ["max"] = UserProfileValidationRules.NickNameMaxLength
                }
            );

        NickName = trimmedNickName;
    }

    public void SetSex(Sex sex)
    {
        Sex = sex;
    }

    private void SetStatus(UserStatus status)
    {
        Status = status;
    }

    public void SetBirthDate(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.UtcDateTime);

        if (birthDate > today)
            throw new ValidationException(
                UserProfileErrorCodes.BirthDateCannotBeFuture,
                "Birth date cannot be in the future",
                new Dictionary<string, object?>
                {
                    ["field"] = "BirthDate"
                }
            );

        if (AgeUtils.Calculate(birthDate, today) < UserProfileValidationRules.MinAge)
            throw new ValidationException(
                UserProfileErrorCodes.AgeCannotBeLessThan,
                $"Age cannot be less than {UserProfileValidationRules.MinAge} years",
                new Dictionary<string, object?>
                {
                    ["field"] = "Age",
                    ["min"] = UserProfileValidationRules.MinAge
                }
            );

        if (AgeUtils.Calculate(birthDate, today) > UserProfileValidationRules.MaxAge)
            throw new ValidationException(
                UserProfileErrorCodes.AgeCannotExceed,
                $"Age cannot exceed than {UserProfileValidationRules.MaxAge} years",
                new Dictionary<string, object?>
                {
                    ["field"] = "Age",
                    ["max"] = UserProfileValidationRules.MaxAge
                }
            );

        BirthDate = birthDate;
    }


    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }

    public void Archived()
    {
        if (Status == UserStatus.Archived)
        {
            throw new ValidationException(
                UserProfileErrorCodes.ArchiveNotAllowed,
                "Cannot archive this user profile",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                }
            );
        }

        Status = UserStatus.Archived;
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
        {
            throw new ValidationException(
                UserProfileErrorCodes.ActivateNotAllowed,
                "Cannot activate this user profile",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                }
            );
        }

        Status = UserStatus.Active;
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
