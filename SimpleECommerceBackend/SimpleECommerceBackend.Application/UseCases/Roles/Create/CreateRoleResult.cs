namespace SimpleECommerceBackend.Application.UseCases.Roles.Create;

public sealed class CreateRoleResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}