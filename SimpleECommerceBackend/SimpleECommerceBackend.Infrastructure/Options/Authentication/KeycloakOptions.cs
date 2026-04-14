namespace SimpleECommerceBackend.Infrastructure.Options.Authentication;

public class KeycloakOptions
{
    public const string SectionName = "KeycloakOptions";

    public string Realm { get; init; } = null!;
    public string AuthServerUrl { get; init; } = null!;
    public string Resource { get; init; } = null!;
    public int TimeoutSeconds { get; init; }
}