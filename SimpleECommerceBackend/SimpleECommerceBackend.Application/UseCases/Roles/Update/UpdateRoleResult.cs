namespace SimpleECommerceBackend.Application.UseCases.Roles.Update;

public sealed class UpdateRoleResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}