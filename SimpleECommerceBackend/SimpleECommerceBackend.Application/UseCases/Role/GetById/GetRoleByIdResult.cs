namespace SimpleECommerceBackend.Application.UseCases.Role.GetById;

public sealed class GetRoleByIdResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}