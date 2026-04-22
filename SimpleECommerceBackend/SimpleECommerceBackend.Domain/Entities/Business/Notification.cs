using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Notification : Entity, ICreatedTrackable
{
    private Notification()
    {
    }

    private Notification(Guid userId, string message)
    {
        SetId(Guid.NewGuid());
        SetUserId(userId);
        SetMessage(message);
    }

    public Guid UserId { get; private set; }
    public UserProfile? User { get; private set; }

    public string Message { get; private set; } = null!;
    public bool IsRead { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static Notification Create(Guid userId, string message)
    {
        return new Notification(userId, message);
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException(
                NotificationErrorCodes.UserIdRequired,
                "User ID is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "UserId"
                }
            );

        UserId = userId;
    }

    private void SetMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ValidationException(
                NotificationErrorCodes.MessageRequired,
                "Message is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Message"
                }
            );

        var trimmedMessage = message.Trim();

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

        Message = trimmedMessage;
    }

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