using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SimpleECommerceBackend.Infrastructure.Persistence;

public sealed class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
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

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}