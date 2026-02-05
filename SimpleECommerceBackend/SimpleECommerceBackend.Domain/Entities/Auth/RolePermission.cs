namespace SimpleECommerceBackend.Domain.Entities.Auth;

public class RolePermission : EntityBase
{
    private RolePermission()
    {
    }

    private RolePermission(Guid roleId, Guid permissionId)
    {
    }

    public Guid RoleId { get; private set; }
    public Role? Role { get; private set; }

    public Guid PermissionId { get; private set; }
    public Permission? Permission { get; private set; }

    public void SetRoleId(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID is required");

        RoleId = roleId;
    }
}