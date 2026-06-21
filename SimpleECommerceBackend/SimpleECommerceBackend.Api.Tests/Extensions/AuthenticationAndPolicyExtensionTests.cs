using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using SimpleECommerceBackend.Api.Authentication;
using SimpleECommerceBackend.Api.Extensions;

namespace SimpleECommerceBackend.Api.Tests.Extensions;

public class AuthenticationAndPolicyExtensionTests
{
    [Fact]
    public void AddKeycloakAuthentication_ShouldConfigureSessionSchemeAsDefault()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var environment = CreateEnvironment(Environments.Development);

        services.AddKeycloakAuthentication(configuration, environment);

        using var serviceProvider = services.BuildServiceProvider();
        var authenticationOptions = serviceProvider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        authenticationOptions.DefaultAuthenticateScheme.Should().Be(AppAuthenticationDefaults.SessionScheme);
        authenticationOptions.DefaultChallengeScheme.Should().Be(AppAuthenticationDefaults.SessionScheme);
        authenticationOptions.DefaultForbidScheme.Should().Be(AppAuthenticationDefaults.SessionScheme);
    }

    [Fact]
    public void AddApiAuthorization_ShouldRequireAuthenticatedUsers()
    {
        var services = new ServiceCollection();

        services.AddApiAuthorization();

        using var serviceProvider = services.BuildServiceProvider();
        var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        authorizationOptions.DefaultPolicy.Requirements.Should().ContainSingle(requirement =>
            requirement is DenyAnonymousAuthorizationRequirement);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:FrontendBaseUrl"] = "https://dev-ecommerce.example.com",
                ["Auth:PostLoginRedirectPath"] = "/",
                ["Auth:PostLogoutRedirectPath"] = "/login",
                ["Auth:SessionCookieName"] = "APP_SESSION_ID",
                ["Auth:CsrfCookieName"] = "XSRF-TOKEN",
                ["Auth:CsrfHeaderName"] = "X-CSRF-TOKEN",
                ["Auth:SessionExpireMinutes"] = "60",
                ["Auth:OAuthStateExpireMinutes"] = "10",
                ["Auth:CookieSecure"] = "true",
                ["Auth:CookieSameSite"] = "None",
                ["Keycloak:Authority"] = "https://dev-identity-provider.example.com/realms/SimpleECommerce",
                ["Keycloak:BaseUrl"] = "https://dev-identity-provider.example.com",
                ["Keycloak:Realm"] = "SimpleECommerce",
                ["Keycloak:ClientId"] = "simple-e-commerce",
                ["Keycloak:ClientSecret"] = "secret",
                ["Keycloak:AuthorizationEndpoint"] = "https://dev-identity-provider.example.com/realms/SimpleECommerce/protocol/openid-connect/auth",
                ["Keycloak:TokenEndpoint"] = "https://dev-identity-provider.example.com/realms/SimpleECommerce/protocol/openid-connect/token",
                ["Keycloak:UserInfoEndpoint"] = "https://dev-identity-provider.example.com/realms/SimpleECommerce/protocol/openid-connect/userinfo",
                ["Keycloak:EndSessionEndpoint"] = "https://dev-identity-provider.example.com/realms/SimpleECommerce/protocol/openid-connect/logout",
                ["Keycloak:CallbackPath"] = "/api/v1.0/auth/login/callback",
                ["Keycloak:RedirectUri"] = "https://dev-api-ecommerce.example.com/api/v1.0/auth/login/callback",
                ["Keycloak:Scopes:0"] = "openid",
                ["Keycloak:Scopes:1"] = "profile",
                ["Keycloak:Scopes:2"] = "email"
            })
            .Build();
    }

    private static IWebHostEnvironment CreateEnvironment(string environmentName)
    {
        return new TestHostEnvironment
        {
            EnvironmentName = environmentName
        };
    }

    private sealed class TestHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "SimpleECommerceBackend.Api.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
