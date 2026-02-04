namespace SimpleECommerceBackend.Domain.Entities.Auth;

public class RolePermission : EntityBase
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}