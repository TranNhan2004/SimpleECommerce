using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<IReadOnlyList<Notification>> FindByUserIdAsync(Guid userId, bool trackChanges = false)
    {
        return await base.FindAllByConditionAsync(
            q => q
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt),
            trackChanges
        );
    }

    public async Task<IReadOnlyList<Notification>> FindUnreadByUserIdAsync(Guid userId, bool trackChanges = false)
    {
        return await base.FindAllByConditionAsync(
            q => q
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt),
            trackChanges
        );
    }
}