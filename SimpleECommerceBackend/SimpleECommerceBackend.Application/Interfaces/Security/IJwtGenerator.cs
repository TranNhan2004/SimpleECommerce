using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Interfaces.Security;

public enum TokenType
{
    AccessToken = 0,
    RefreshToken = 1,
    AccountVerificationToken = 2,
    PasswordResetToken = 3
}

public class ClaimsInfo
{
    public string Email { get; init; } = null!;
    public Role? Role { get; init; }
    public TokenType TokenType { get; init; }
}

public interface IJwtGenerator
{
    string GenerateAccessToken(string email, Role role);
    string GenerateRefreshToken(string email, Role role);
    string GenerateAccountVerificationToken(string email);
    string GeneratePasswordResetToken(string email);
    ClaimsInfo ValidateToken(string token);
}