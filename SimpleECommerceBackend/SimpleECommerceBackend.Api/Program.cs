using DotNetEnv;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi;
using SimpleECommerceBackend.Api.Extensions;
using SimpleECommerceBackend.Api.Mapping;
using SimpleECommerceBackend.Application;
using SimpleECommerceBackend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

if (env.IsDevelopment())
{
    var envFile = Path.Combine(
        env.ContentRootPath,
        ".env.development"
    );

    if (File.Exists(envFile))
        Env.Load(envFile);
}


builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var mapsterConfig = TypeAdapterConfig.GlobalSettings;
mapsterConfig.Scan(typeof(RoleMappingConfig).Assembly);
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

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

        // 4. URL segment (phổ biến nhất)
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
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Simple E-Commerce API",
        Version = "v1"
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();
app.MapControllers();
app.Run();