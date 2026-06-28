using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Entities.Uam;

public class User : EntityBase
{
    public User()
    {
    }

    private Guid _keycloakSubjectId;
    private string _email = null!;
    private string _firstName = null!;
    private string _lastName = null!;
    private string? _nickName;
    private Sex _sex;
    private UserStatus _status;
    private DateOnly _birthDate;
    private string? _avatarUrl;
    private bool _isEmailVerified;
    private DateTimeOffset? _emailVerifiedAt;
    private DateTimeOffset? _lastLoginAt;

    public Guid KeycloakSubjectId
    {
        get => _keycloakSubjectId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    UserErrorCodes.KeycloakSubjectIdInvalid,
                    "Keycloak subject id is invalid",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "KeycloakSubjectId"
                    }
                );

            _keycloakSubjectId = value;
        }
    }

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
                    UserErrorCodes.FirstNameRequired,
                    "First name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "FirstName"
                    }
                );

            var trimmedFirstName = value.Trim();

            if (trimmedFirstName.Length > UserProfileValidationRules.FirstNameMaxLength)
                throw new ValidationException(
                    UserErrorCodes.FirstNameMaxLengthExceeded,
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
                    UserErrorCodes.LastNameRequired,
                    "Last name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "LastName"
                    }
                );

            var trimmedLastName = value.Trim();

            if (trimmedLastName.Length > UserProfileValidationRules.LastNameMaxLength)
                throw new ValidationException(
                    UserErrorCodes.LastNameMaxLengthExceeded,
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
                    UserErrorCodes.NickNameMustNotBeBlank,
                    "Nick name is not blank",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "NickName"
                    }
                );

            var trimmedNickName = value.Trim();

            if (trimmedNickName.Length > UserProfileValidationRules.NickNameMaxLength)
                throw new ValidationException(
                    UserErrorCodes.NickNameMaxLengthExceeded,
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
                    UserErrorCodes.BirthDateCannotBeFuture,
                    "Birth date cannot be in the future",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "BirthDate"
                    }
                );

            if (AgeUtils.Calculate(value, today) < UserProfileValidationRules.MinAge)
                throw new ValidationException(
                    UserErrorCodes.AgeCannotBeLessThan,
                    $"Age cannot be less than {UserProfileValidationRules.MinAge} years",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Age",
                        ["min"] = UserProfileValidationRules.MinAge
                    }
                );

            if (AgeUtils.Calculate(value, today) > UserProfileValidationRules.MaxAge)
                throw new ValidationException(
                    UserErrorCodes.AgeCannotExceed,
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

    public bool IsEmailVerified
    {
        get => _isEmailVerified;
        set => _isEmailVerified = value;
    }

    public DateTimeOffset? EmailVerifiedAt
    {
        get => _emailVerifiedAt;
        set => _emailVerifiedAt = value;
    }

    public DateTimeOffset? LastLoginAt
    {
        get => _lastLoginAt;
        set => _lastLoginAt = value;
    }
}
