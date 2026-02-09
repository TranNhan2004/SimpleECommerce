using DotNetEnv;
using Microsoft.Extensions.Configuration;

namespace SimpleECommerceBackend.Infrastructure.Persistence;

public static class DbConnectionStringBuilder
{
    public static string Build()
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";

        if (environment == "Development")
            Env.Load(".env.development");
        else
            Env.Load(".env.production");

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            $"Server={configuration["DB_HOST"]},{configuration["DB_PORT"]};" +
            $"Database={configuration["DB_NAME"]};" +
            $"User Id={configuration["DB_USER"]};" +
            $"Password={configuration["DB_PASSWORD"]};" +
            $"TrustServerCertificate=True;";

        return connectionString;
    }
}