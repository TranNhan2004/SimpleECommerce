using System.Security.Cryptography;
using System.Text;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Utils;

public class TokenUtils
{
    public static string CreateVerificationToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

    public static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    public static string NormalizeToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new BusinessException("Verification token is required");

        return token.Trim();
    }
}