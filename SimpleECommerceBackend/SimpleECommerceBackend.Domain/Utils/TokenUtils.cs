using System.Security.Cryptography;
using System.Text;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Utils;

public static class TokenUtils
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
            throw new ValidationException(
                TokenErrorCodes.VerificationTokenRequired,
                "Verification token is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "VerificationToken"
                }
            );

        return token.Trim();
    }
}