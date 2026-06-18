using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;
using SimpleECommerceBackend.Api.Extensions;

namespace SimpleECommerceBackend.Api.Tests.Extensions;

public class AuthenticationAndPolicyExtensionTests
{
    [Fact]
    public void AddKeycloakAuthentication_ShouldNotConfigureDefaultAuthenticateScheme()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("http://localhost:8080/");
        var environment = CreateEnvironment(Environments.Development);

        services.AddKeycloakAuthentication(configuration, environment);

        using var serviceProvider = services.BuildServiceProvider();
        var authenticationOptions = serviceProvider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        var jwtBearerOptions = serviceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        authenticationOptions.DefaultAuthenticateScheme.Should().BeNull();
        authenticationOptions.DefaultChallengeScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        authenticationOptions.DefaultForbidScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        jwtBearerOptions.RequireHttpsMetadata.Should().BeFalse();
    }

    [Fact]
    public void AddKeycloakAuthentication_ShouldAllowHttpMetadata_ForHttpAuthorityOutsideDevelopment()
    {
        var services = new ServiceCollection();
        var configuration = CreateConfiguration("http://keycloak:8080/");
        var environment = CreateEnvironment(Environments.Production);

        services.AddKeycloakAuthentication(configuration, environment);

        using var serviceProvider = services.BuildServiceProvider();
        var jwtBearerOptions = serviceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        jwtBearerOptions.RequireHttpsMetadata.Should().BeFalse();
    }

    [Fact]
    public void AddApiAuthorization_ShouldUseBearerSchemeInDefaultPolicy()
    {
        var services = new ServiceCollection();

        services.AddApiAuthorization();

        using var serviceProvider = services.BuildServiceProvider();
        var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        authorizationOptions.DefaultPolicy.AuthenticationSchemes.Should().ContainSingle(JwtBearerDefaults.AuthenticationScheme);
        authorizationOptions.DefaultPolicy.Requirements.Should().ContainSingle(requirement =>
            requirement is DenyAnonymousAuthorizationRequirement);
    }

    private static IConfiguration CreateConfiguration(string authServerUrl)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["KeycloakOptions:Realm"] = "SimpleECommerce",
                ["KeycloakOptions:AuthServerUrl"] = authServerUrl,
                ["KeycloakOptions:Resource"] = "simple-e-commerce-backend",
                ["KeycloakOptions:TimeoutSeconds"] = "30"
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
