using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Interfaces.Security;

public sealed class ClaimsInfo
{
    public Guid UserId { get; init; }
    public string? Email { get; init; }
    public Role? Role { get; init; }
    public string? TokenType { get; init; }
}

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string email, Role role);
    string GenerateRefreshToken();
    string GenerateAccountVerificationToken(Guid userId);
    string GeneratePasswordResetToken(Guid userId);
    ClaimsInfo ValidateToken(string token);
}