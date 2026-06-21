namespace SimpleECommerceBackend.Infrastructure.Options.Authentication;

public class KeycloakBffOptions
{
    public const string SectionName = "Keycloak";

    public string Authority { get; init; } = null!;
    public string BaseUrl { get; init; } = null!;
    public string Realm { get; init; } = null!;
    public string ClientId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    public string AuthorizationEndpoint { get; init; } = null!;
    public string TokenEndpoint { get; init; } = null!;
    public string UserInfoEndpoint { get; init; } = null!;
    public string EndSessionEndpoint { get; init; } = null!;
    public string CallbackPath { get; init; } = null!;
    public string RedirectUri { get; init; } = null!;
    public IReadOnlyList<string> Scopes { get; init; } = [];
}
