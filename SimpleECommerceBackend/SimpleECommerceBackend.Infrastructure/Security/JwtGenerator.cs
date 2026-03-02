using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Security;

public class JwtGenerator : IJwtGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtGenerator(IConfiguration configuration)
    {
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
    }

    public string GenerateAccessToken(string email, Role role)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, role.ToString()),
            new("token_type", TokenType.AccessToken.ToString())
        };

        return GenerateToken(claims, _jwtSettings.AccessTokenExpirationMinutes);
    }

    public string GenerateRefreshToken(string email, Role role)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, role.ToString()),
            new("token_type", TokenType.RefreshToken.ToString())
        };

        return GenerateToken(claims, _jwtSettings.RefreshTokenExpirationMinutes);
    }

    public string GenerateAccountVerificationToken(string email)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_type", TokenType.AccountVerificationToken.ToString())
        };

        return GenerateToken(claims, _jwtSettings.AccountVerificationTokenExpirationMinutes);
    }

    public string GeneratePasswordResetToken(string email)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_type", TokenType.PasswordResetToken.ToString())
        };

        return GenerateToken(claims, _jwtSettings.AccountVerificationTokenExpirationMinutes);
    }

    public ClaimsInfo ValidateToken(string token)
    {
        try
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
                Email = principal.FindFirst(JwtRegisteredClaimNames.Email)!.Value,

                Role = Enum.TryParse<Role>(principal.FindFirst(ClaimTypes.Role)?.Value, out var role)
                    ? role
                    : null,

                TokenType = Enum.TryParse<TokenType>(principal.FindFirst("token_type")?.Value, out var tokenType)
                    ? tokenType
                    : default,
            };
        } 
        catch (SecurityTokenExpiredException ex)
        {
            throw new UnauthorizedException("Token expired", ex);
        }
        catch (SecurityTokenException ex)
        {
            throw new UnauthorizedException("Invalid token", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Token validation failed unexpectedly", ex);
        }
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