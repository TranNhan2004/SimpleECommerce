namespace SimpleECommerceBackend.Infrastructure.Services.Keycloak;

public class KeycloakSettings
{
    public string Realm { get; init; } = null!;
    public string AuthServerUrl { get; init; } = null!;
    public string Resource { get; init; } = null!;
    public KeycloakCredentials Credentials { get; init; } = null!;
    public string TokenEndpoint { get; init; } = null!;
    public string UserInfoEndpoint { get; init; } = null!;
    public string IntrospectionEndpoint { get; init; } = null!;
    public string AdminUrl { get; init; } = null!;
    public bool VerifyTokenAudience { get; init; }
    public int TimeoutSeconds { get; init; }
}

public class KeycloakCredentials
{
    public string Secret { get; init; } = null!;
}
