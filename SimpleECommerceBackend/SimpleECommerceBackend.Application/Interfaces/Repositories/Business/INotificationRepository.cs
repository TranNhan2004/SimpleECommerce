using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<IReadOnlyList<Notification>> FindByUserIdAsync(Guid userId, bool trackChanges = false);
    Task<IReadOnlyList<Notification>> FindUnreadByUserIdAsync(Guid userId, bool trackChanges = false);
}