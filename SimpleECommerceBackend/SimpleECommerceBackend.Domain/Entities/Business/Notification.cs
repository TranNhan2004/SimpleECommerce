using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Notification : EntityBase, ICreatedTime
{
    private Notification()
    {
    }

    private Notification(Guid userId, string message)
    {
        SetUserId(userId);
        SetMessage(message);
        IsRead = false;
    }

    public Guid UserId { get; private set; }
    public UserProfile? User { get; private set; }

    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static Notification Create(Guid userId, string message)
    {
        return new Notification(userId, message);
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID is required");

        UserId = userId;
    }

    private void SetMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new DomainException("Message is required");

        var trimmedMessage = message.Trim();

        if (trimmedMessage.Length > NotificationConstants.MessageMaxLength)
            throw new DomainException($"Message cannot exceed {NotificationConstants.MessageMaxLength} characters");

        Message = trimmedMessage;
    }

    public void MarkAsRead()
    {
        if (IsRead)
            throw new DomainException("Notification is already marked as read");

        IsRead = true;
    }
}