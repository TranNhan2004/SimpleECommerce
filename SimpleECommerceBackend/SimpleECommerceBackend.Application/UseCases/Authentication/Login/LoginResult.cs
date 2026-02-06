namespace SimpleECommerceBackend.Application.UseCases.Authentication.Login;

public sealed class LoginResult
{
    public Guid CredentialId { get; init; }
    public string Email { get; init; } = string.Empty;
    public Guid RoleId { get; init; }
}