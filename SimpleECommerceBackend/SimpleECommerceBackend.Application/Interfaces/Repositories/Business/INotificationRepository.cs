using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> FindByUserIdAsync(Guid userId);
    Task<IReadOnlyList<Notification>> FindUnreadByUserIdAsync(Guid userId);
    Task<Notification?> FindByIdAsync(Guid id);
    Notification Add(Notification notification);
    Notification Update(Notification notification);
}
