using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
    }

    public string GenerateAccessToken(Guid userId, string email, Role role)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, role.ToString())
        };

        return GenerateToken(claims, _jwtSettings.AccessTokenExpirationMinutes);
    }

    public string GenerateRefreshToken()
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_type", "refresh_token")
        };

        return GenerateToken(claims, _jwtSettings.RefreshTokenExpirationMinutes);
    }

    public string GenerateAccountVerificationToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_type", "verfication_token")
        };

        return GenerateToken(claims, _jwtSettings.AccountVerificationTokenExpirationMinutes);
    }

    public string GeneratePasswordResetToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_type", "password_reset_token")
        };

        return GenerateToken(claims, _jwtSettings.AccountVerificationTokenExpirationMinutes);
    }

    public ClaimsInfo ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, parameters, out _);


        return new ClaimsInfo
        {
            UserId = Guid.TryParse(
                principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty,
                out var userId
            )
                ? userId
                : Guid.Empty,

            Email = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,

            Role = Enum.TryParse<Role>(principal.FindFirst(ClaimTypes.Role)?.Value, out var role)
                ? role
                : default,

            TokenType = principal.FindFirst("token_type")?.Value
        };
    }

    private string GenerateToken(IEnumerable<Claim> claims, int expiryMinutes)
    {
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}