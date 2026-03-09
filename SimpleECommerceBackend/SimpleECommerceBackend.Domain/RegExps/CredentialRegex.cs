using System.Text.RegularExpressions;

namespace SimpleECommerceBackend.Domain.RegExps;

public static partial class CredentialRegex
{
    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    public static partial Regex EmailPattern();

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
    public static partial Regex PasswordPattern();
}