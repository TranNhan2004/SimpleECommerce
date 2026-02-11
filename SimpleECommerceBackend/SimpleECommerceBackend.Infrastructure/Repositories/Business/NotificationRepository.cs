using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

[AutoConstructor]
public sealed partial class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _db;

    public async Task<IEnumerable<Notification>> FindByUserIdAsync(Guid userId)
    {
        return await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> FindUnreadByUserIdAsync(Guid userId)
    {
        return await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> FindByIdAsync(Guid id)
    {
        return await _db.Notifications.FindAsync(id);
    }

    public Notification Add(Notification notification)
    {
        _db.Notifications.Add(notification);
        return notification;
    }

    public Notification Update(Notification notification)
    {
        _db.Notifications.Update(notification);
        return notification;
    }
}
