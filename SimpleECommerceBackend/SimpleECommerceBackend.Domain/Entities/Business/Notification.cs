using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Notification : EntityBase, ICreatedTrackable
{
    public Notification()
    {
    }

    // private Notification(Guid userId, string message)
    // {
    //     Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
    //     UserId = userId;
    //     Message = message;
    // }

    private Guid _userId;
    private string _message = null!;

    public Guid UserId
    {
        get => _userId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    NotificationErrorCodes.UserIdRequired,
                    "User ID is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "UserId"
                    }
                );

            _userId = value;
        }
    }

    public User? User { get; private set; }

    public string Message
    {
        get => _message;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    NotificationErrorCodes.MessageRequired,
                    "Message is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Message"
                    }
                );

            var trimmedMessage = value.Trim();

            if (trimmedMessage.Length > NotificationValidationRules.MessageMaxLength)
                throw new ValidationException(
                    NotificationErrorCodes.MessageMaxLengthExceeded,
                    $"Message cannot exceed {NotificationValidationRules.MessageMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Message",
                        ["max"] = NotificationValidationRules.MessageMaxLength
                    }
                );

            _message = trimmedMessage;
        }
    }

    public bool IsRead { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public void MarkAsRead()
    {
        if (IsRead)
            throw new ValidationException(
                NotificationErrorCodes.AlreadyMarkedAsRead,
                "Notification is already marked as read",
                new Dictionary<string, object?>
                {
                    ["field"] = "Notification",
                    ["state"] = "marked as read"
                }
            );

        IsRead = true;
    }
}
