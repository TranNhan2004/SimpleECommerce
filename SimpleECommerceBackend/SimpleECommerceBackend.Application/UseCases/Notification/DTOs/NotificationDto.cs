namespace SimpleECommerceBackend.Application.UseCases.Notification.DTOs;

public record NotificationDto(
    Guid Id,
    string Message,
    bool IsRead,
    DateTimeOffset CreatedAt
);
