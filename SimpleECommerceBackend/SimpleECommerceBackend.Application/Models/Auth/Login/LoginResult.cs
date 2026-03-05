using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Auth.Login;

public class LoginResult
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }
    public string Role { get; init; } = null!;
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
}
