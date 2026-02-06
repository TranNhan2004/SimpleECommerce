using SimpleECommerceBackend.Application.Interfaces.Security;

namespace SimpleECommerceBackend.Infrastructure.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(
            plainPassword,
            WorkFactor
        );
    }

    public bool Verify(string plainPassword, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
    }
}