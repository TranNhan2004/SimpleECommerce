using MediatR;
using SimpleECommerceBackend.Application.UseCases.Notification.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Notification.Queries.GetMyNotifications;

public record GetMyNotificationsQuery(Guid UserId) : IRequest<IEnumerable<NotificationDto>>;

[AutoConstructor]
public partial class GetMyNotificationsHandler : IRequestHandler<GetMyNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;

    public async Task<IEnumerable<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.FindByUserIdAsync(request.UserId);
        
        return notifications.Select(n => new NotificationDto(
            n.Id,
            n.Message,
            n.IsRead,
            n.CreatedAt
        ));
    }
}
