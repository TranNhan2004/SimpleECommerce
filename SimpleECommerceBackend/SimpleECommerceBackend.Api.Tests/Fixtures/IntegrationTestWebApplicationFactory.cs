using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Api.Tests.Fixtures;

/// <summary>
/// Custom WebApplicationFactory for integration tests that properly configures
/// the test environment with database and Keycloak settings.
/// </summary>
public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment to Development to load appsettings.Development.json
        builder.UseEnvironment("Development");

        // Resolve the API project from the compiled test assembly so the factory works
        // regardless of which directory dotnet test is launched from.
        var apiProjectPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "SimpleECommerceBackend.Api")
        );

        builder.UseContentRoot(apiProjectPath);

        // Explicitly configure to load appsettings files
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            // The configuration is already loaded by the Program.cs
            // We just need to ensure the database is ready for testing

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();
        });
    }
}

