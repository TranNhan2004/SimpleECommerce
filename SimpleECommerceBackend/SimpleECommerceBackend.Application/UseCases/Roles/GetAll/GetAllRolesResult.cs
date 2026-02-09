namespace SimpleECommerceBackend.Application.UseCases.Roles.GetAll;

public sealed class GetAllRolesResult
{
    public IEnumerable<RoleItem> Roles { get; init; } = [];

    public sealed class RoleItem
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}