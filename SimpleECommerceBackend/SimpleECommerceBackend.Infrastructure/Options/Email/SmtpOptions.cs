namespace SimpleECommerceBackend.Infrastructure.Options.Email;

public class SmtpOptions
{
    public const string SectionName = "SmtpOptions";

    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public bool UseSsl { get; init; }
    public string From { get; init; } = null!;
}