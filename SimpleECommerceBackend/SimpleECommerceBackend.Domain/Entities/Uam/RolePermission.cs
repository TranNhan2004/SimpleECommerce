namespace SimpleECommerceBackend.Domain.Entities.Uam;

public class RolePermission : EntityBase
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public Role? Role { get; private set; }
    public Permission? Permission { get; private set; }
}
