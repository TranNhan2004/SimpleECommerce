using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Notification.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid NotificationId) : IRequest;

[AutoConstructor]
public partial class MarkNotificationReadHandler : IRequestHandler<MarkNotificationReadCommand>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FindByIdAsync(request.NotificationId);
        if (notification is null)
            throw new KeyNotFoundException($"Notification {request.NotificationId} not found");

        // Need logic to check user access? 
        // Command doesn't have UserId. Usually validated by finding notification that belongs to user.
        // If specific ID given, assume authorized or handled by middleware/controller.
        // Domain verification: Notification.MarkAsRead().
        
        notification.MarkAsRead();
        _notificationRepository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
