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
        Email = CredentialUtils.ValidateEmail(email);
    }

    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            throw new ValidationException(
                UserProfileErrorCode.FirstNameRequired,
                "First name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "FirstName"
                }
            );

        var trimmedFirstName = firstName.Trim();

        if (trimmedFirstName.Length > UserProfileConstants.FirstNameMaxLength)
            throw new ValidationException(
                UserProfileErrorCode.FirstNameMaxLengthExceeded,
                $"First name cannot exceed {UserProfileConstants.FirstNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "FirstName",
                    ["max"] = UserProfileConstants.FirstNameMaxLength
                }
            );

        FirstName = trimmedFirstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            throw new ValidationException(
                UserProfileErrorCode.LastNameRequired,
                "Last name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "LastName"
                }
            );

        var trimmedLastName = lastName.Trim();

        if (trimmedLastName.Length > UserProfileConstants.LastNameMaxLength)
            throw new ValidationException(
                UserProfileErrorCode.LastNameMaxLengthExceeded,
                $"Last name cannot exceed {UserProfileConstants.LastNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "LastName",
                    ["max"] = UserProfileConstants.LastNameMaxLength
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
                UserProfileErrorCode.NickNameMustNotBeBlank,
                "Nick name is not blank",
                new Dictionary<string, object?>
                {
                    ["field"] = "NickName"
                }
            );

        var trimmedNickName = nickName.Trim();

        if (trimmedNickName.Length > UserProfileConstants.NickNameMaxLength)
            throw new ValidationException(
                UserProfileErrorCode.NickNameMaxLengthExceeded,
                $"Nick name cannot exceed {UserProfileConstants.NickNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "NickName",
                    ["max"] = UserProfileConstants.NickNameMaxLength
                }
            );

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
            throw new ValidationException(
                UserProfileErrorCode.BirthDateCannotBeFuture,
                "Birth date cannot be in the future",
                new Dictionary<string, object?>
                {
                    ["field"] = "BirthDate"
                }
            );

        if (AgeUtils.Calculate(birthDate, today) < UserProfileConstants.MinAge)
            throw new ValidationException(
                UserProfileErrorCode.AgeCannotBeLessThan,
                $"Age cannot be less than {UserProfileConstants.MinAge} years",
                new Dictionary<string, object?>
                {
                    ["field"] = "Age",
                    ["min"] = UserProfileConstants.MinAge
                }
            );

        if (AgeUtils.Calculate(birthDate, today) > UserProfileConstants.MaxAge)
            throw new ValidationException(
                UserProfileErrorCode.AgeCannotExceed,
                $"Age cannot exceed than {UserProfileConstants.MaxAge} years",
                new Dictionary<string, object?>
                {
                    ["field"] = "Age",
                    ["max"] = UserProfileConstants.MaxAge
                }
            );

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