using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> FindByUserIdAsync(Guid userId);
    Task<IEnumerable<Notification>> FindUnreadByUserIdAsync(Guid userId);
    Task<Notification?> FindByIdAsync(Guid id);
    Notification Add(Notification notification);
    Notification Update(Notification notification);
}
