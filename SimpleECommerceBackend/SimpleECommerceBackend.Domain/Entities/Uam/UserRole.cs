using SimpleECommerceBackend.Domain.Entities.Abstracts;

namespace SimpleECommerceBackend.Domain.Entities.Uam;

public class UserRole : EntityBase, ICreatedTrackable
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public User? User { get; private set; }
    public Role? Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
