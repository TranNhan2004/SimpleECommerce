namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class LoginResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }
    public string Role { get; init; } = null!;
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
}