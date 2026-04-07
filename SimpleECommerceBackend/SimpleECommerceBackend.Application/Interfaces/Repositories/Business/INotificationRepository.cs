using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<IReadOnlyList<Notification>> FindByUserIdAsync(Guid userId);
    Task<IReadOnlyList<Notification>> FindUnreadByUserIdAsync(Guid userId);
}