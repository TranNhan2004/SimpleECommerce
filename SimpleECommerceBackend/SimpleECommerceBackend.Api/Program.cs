using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi;
using Serilog;
using SimpleECommerceBackend.Api;
using SimpleECommerceBackend.Api.Extensions;
using SimpleECommerceBackend.Application;
using SimpleECommerceBackend.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddOptions();
    builder.Services.AddAppOptions(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddKeycloakAuthentication(builder.Configuration, builder.Environment);
    builder.Services.AddApiAuthorization();
    builder.Services.AddCustomRateLimiter(builder.Configuration);
    builder.Services.AddControllers();

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;

        // Use 1 in 4 ways to set version
        options.ApiVersionReader = ApiVersionReader.Combine(
            // 1. Query string
            // GET /api/products?api-version=1.0
            new QueryStringApiVersionReader("api-version"),

            // 2. Header
            // GET /api/products
            // Header: X-Version: 1.0
            new HeaderApiVersionReader("X-Version"),

            // 3. Media type (content negotiation)
            // GET /api/products
            // Header: Accept: application/json;ver=1.0
            new MediaTypeApiVersionReader("ver"),

            // 4. URL segment
            // GET /api/v1/products
            new UrlSegmentApiVersionReader()
        );
    });

    builder.Services.AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // Add services to the container.
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Simple E-Commerce API",
            Version = "v1"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter your token below.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseGlobalExceptionHandler();
    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseForwardedHeaders();
    app.UseRateLimiter();

    app.MapControllers().RequireRateLimiting("ip-route");
    // var sqlStatements = Test.GenerateInsertStatements();
    // Console.WriteLine($"Generated SQL Insert Statements: {sqlStatements.Count}");
    // Test.SaveToFile(sqlStatements);
    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly during startup.");
}
finally
{
    await Log.CloseAndFlushAsync();
}
