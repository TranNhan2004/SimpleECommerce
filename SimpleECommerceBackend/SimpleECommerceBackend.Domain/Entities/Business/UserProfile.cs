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
    public UserProfile()
    {
    }

    // [JsonConstructor]
    // private UserProfile(
    //     Guid id,
    //     string email,
    //     string firstName,
    //     string lastName,
    //     string? nickName,
    //     Sex sex,
    //     UserStatus status,
    //     DateOnly birthDate,
    //     string? avatarUrl,
    //     DateTimeOffset createdAt,
    //     DateTimeOffset? updatedAt
    // )
    // {
    //     Id = id;
    //     Email = email;
    //     FirstName = firstName;
    //     LastName = lastName;
    //     NickName = nickName;
    //     Sex = sex;
    //     Status = status;
    //     BirthDate = birthDate;
    //     AvatarUrl = avatarUrl;
    //     CreatedAt = createdAt;
    //     UpdatedAt = updatedAt;
    // }

    private string _email = null!;
    private string _firstName = null!;
    private string _lastName = null!;
    private string? _nickName;
    private Sex _sex;
    private UserStatus _status;
    private DateOnly _birthDate;
    private string? _avatarUrl;

    public string Email
    {
        get => _email;
        set => _email = CredentialUtils.ValidateEmail(value);
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    UserProfileErrorCodes.FirstNameRequired,
                    "First name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "FirstName"
                    }
                );

            var trimmedFirstName = value.Trim();

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

            _firstName = trimmedFirstName;
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    UserProfileErrorCodes.LastNameRequired,
                    "Last name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "LastName"
                    }
                );

            var trimmedLastName = value.Trim();

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

            _lastName = trimmedLastName;
        }
    }

    public string? NickName
    {
        get => _nickName;
        set
        {
            if (value is null)
            {
                _nickName = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    UserProfileErrorCodes.NickNameMustNotBeBlank,
                    "Nick name is not blank",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "NickName"
                    }
                );

            var trimmedNickName = value.Trim();

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

            _nickName = trimmedNickName;
        }
    }

    public Sex Sex
    {
        get => _sex;
        set => _sex = value;
    }

    public UserStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public DateOnly BirthDate
    {
        get => _birthDate;
        set
        {
            var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.UtcDateTime);

            if (value > today)
                throw new ValidationException(
                    UserProfileErrorCodes.BirthDateCannotBeFuture,
                    "Birth date cannot be in the future",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "BirthDate"
                    }
                );

            if (AgeUtils.Calculate(value, today) < UserProfileValidationRules.MinAge)
                throw new ValidationException(
                    UserProfileErrorCodes.AgeCannotBeLessThan,
                    $"Age cannot be less than {UserProfileValidationRules.MinAge} years",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Age",
                        ["min"] = UserProfileValidationRules.MinAge
                    }
                );

            if (AgeUtils.Calculate(value, today) > UserProfileValidationRules.MaxAge)
                throw new ValidationException(
                    UserProfileErrorCodes.AgeCannotExceed,
                    $"Age cannot exceed than {UserProfileValidationRules.MaxAge} years",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Age",
                        ["max"] = UserProfileValidationRules.MaxAge
                    }
                );

            _birthDate = value;
        }
    }

    public string? AvatarUrl
    {
        get => _avatarUrl;
        set => _avatarUrl = value;
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

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
}
