# Phase 4: API Layer Updates

**Status**: ✅ Complete  
**Duration**: 1-2 days  
**Phase Overview**: [KEYCLOAK_IMPLEMENTATION_PLAN.md](./KEYCLOAK_IMPLEMENTATION_PLAN.md)

---

## Table of Contents

1. [Objectives](#objectives)
2. [Prerequisites](#prerequisites)
3. [Implementation Steps](#implementation-steps)
   - [Step 4.1: Update Program.cs](#step-41-update-programcs)
   - [Step 4.2: Update AuthController](#step-42-update-authcontroller)
   - [Step 4.3: Update DTOs](#step-43-update-dtos)
4. [Verification Checklist](#verification-checklist)
5. [Troubleshooting](#troubleshooting)
6. [Next Steps](#next-steps)

---

## Objectives

- ✅ Configure Keycloak authentication in Program.cs
- ✅ Update AuthController to work with Keycloak
- ✅ Update authentication DTOs for Keycloak flow
- ✅ Configure authorization policies for role-based access
- ✅ Update Swagger to support Bearer token authentication
- ✅ Remove custom JWT authentication configuration

---

## Prerequisites

- [x] Phase 1 (Keycloak Setup) completed
  - Client: `simple-e-commerce-backend` configured in Keycloak
  - Realm: `SimpleECommerce` with roles: customer, seller, admin
  - Client scope: `simple-e-commerce-roles` with role mapper
- [x] Phase 2 (Backend Configuration) completed
- [x] Phase 3 (Authentication Service Implementation) completed
- [x] Keycloak services interfaces and implementations created
- [x] Keycloak NuGet packages installed

---

## Implementation Steps

### Step 4.1: Update Program.cs

**File**: `SimpleECommerceBackend.Api/Program.cs`

#### 4.1.1: Add Required Namespaces

Add the following using statements at the top of the file:

```csharp
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
```

#### 4.1.2: Replace Authentication Configuration

Replace the existing JWT authentication configuration with Keycloak authentication:

**Complete updated Program.cs:**

```csharp
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using SimpleECommerceBackend.Api.Extensions;
using SimpleECommerceBackend.Api.Middleware;
using SimpleECommerceBackend.Application;
using SimpleECommerceBackend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var mapsterConfig = TypeAdapterConfig.GlobalSettings;
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// Keycloak Authentication
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, options =>
{
    options.Audience = builder.Configuration["Keycloak:resource"];
    options.RequireHttpsMetadata = false; // Set to true in production
});

// Keycloak Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireCustomerRole", policy =>
        policy.RequireRole("customer"));

    options.AddPolicy("RequireSellerRole", policy =>
        policy.RequireRole("seller"));

    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("admin"));
});

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver"),
        new UrlSegmentApiVersionReader()
    );
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Simple E-Commerce API",
        Version = "v1"
    });

    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
```

#### 4.1.3: Key Changes Explained

**Authentication Configuration:**

- Replaced `AddAuthentication().AddJwtBearer()` with `AddKeycloakWebApiAuthentication()`
- Configured audience from Keycloak settings
- Set `RequireHttpsMetadata = false` for development (change to `true` in production)

**Authorization Policies:**

- Created three role-based policies: `RequireCustomerRole`, `RequireSellerRole`, `RequireAdminRole`
- These policies will be used with `[Authorize(Policy = "...")]` attribute on controllers

---

### Step 4.2: Update AuthController

**File**: `SimpleECommerceBackend.Api/Controllers/AuthController.cs`

#### 4.2.1: Update Controller Implementation

Replace the entire controller with the following code:

```csharp
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleECommerceBackend.Api.DTOs.Auth;
using SimpleECommerceBackend.Api.DTOs.Errors;
using SimpleECommerceBackend.Application.UseCases.Auth.Login;
using SimpleECommerceBackend.Application.UseCases.Auth.RefreshToken;
using SimpleECommerceBackend.Application.UseCases.Auth.Register;

namespace SimpleECommerceBackend.Api.Controllers;

[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
[ApiController]
[AutoConstructor]
public partial class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Role
        );

        var result = await _sender.Send(command);
        var response = _mapper.Map<RegisterResult, RegisterResponse>(result);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _sender.Send(command);
        var response = _mapper.Map<LoginResult, LoginResponse>(result);
        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _sender.Send(command);
        var response = _mapper.Map<RefreshTokenResult, RefreshTokenResponse>(result);
        return Ok(response);
    }
}
```

#### 4.2.2: Key Changes Explained

**Simplified Implementation:**

- Controller now only acts as a thin API layer
- All Keycloak logic is delegated to the use case handlers
- Same endpoints but with different backend implementation
- No change to the API contract from the client perspective

---

### Step 4.3: Update DTOs

#### 4.3.1: Update RegisterRequest

**File**: `SimpleECommerceBackend.Api/DTOs/Auth/RegisterRequest.cs`

```csharp
namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class RegisterRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Role { get; init; } = "customer"; // Default role
}
```

#### 4.3.2: Update LoginRequest

**File**: `SimpleECommerceBackend.Api/DTOs/Auth/LoginRequest.cs`

```csharp
namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class LoginRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
```

#### 4.3.3: Create RefreshTokenRequest

**File**: `SimpleECommerceBackend.Api/DTOs/Auth/RefreshTokenRequest.cs`

```csharp
namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; init; } = null!;
}
```

#### 4.3.4: Create RefreshTokenResponse

**File**: `SimpleECommerceBackend.Api/DTOs/Auth/RefreshTokenResponse.cs`

```csharp
namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class RefreshTokenResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
}
```

#### 4.3.5: Update RegisterResponse

**File**: `SimpleECommerceBackend.Api/DTOs/Auth/RegisterResponse.cs`

```csharp
namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class RegisterResponse
{
    public string Email { get; init; } = null!;
    public string Message { get; init; } = "User registered successfully";
}
```

#### 4.3.6: Update LoginResponse

**File**: `SimpleECommerceBackend.Api/DTOs/Auth/LoginResponse.cs`

```csharp
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class LoginResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }
    public string Role { get; init; } = null!;
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
}
```

#### 4.3.7: Key DTO Changes

**What Changed:**

- `RefreshTokenRequest` and `RefreshTokenResponse` are new DTOs
- `LoginResponse` now includes `RefreshToken` and `ExpiresIn` fields from Keycloak
- All DTOs remain compatible with existing API contracts

---

## Verification Checklist

After completing this phase, verify the following:

- [x] Program.cs uses Keycloak authentication configuration
- [x] Program.cs defines authorization policies for customer, seller, and admin roles
- [x] AuthController has register, login, and refresh endpoints
- [x] All DTOs are properly defined and match the API contract
- [x] Swagger UI shows Bearer token authentication option
- [x] Application builds without errors
- [x] No compilation errors in API layer

---

## Troubleshooting

### Issue: "AddKeycloakWebApiAuthentication not found"

**Solution:**

- Ensure `Keycloak.AuthServices.Authentication` NuGet package is installed
- Add `using Keycloak.AuthServices.Authentication;` to Program.cs
- Rebuild the solution

### Issue: "Keycloak configuration section not found"

**Solution:**

- Verify `appsettings.json` has the `Keycloak` section (from Phase 2)
- Check that the configuration keys match exactly

### Issue: "Authorization policies not working"

**Solution:**

- Ensure `UseAuthentication()` is called before `UseAuthorization()` in Program.cs
- Verify role names in policies match Keycloak realm roles

### Issue: "Swagger doesn't show authentication"

**Solution:**

- Verify `AddSecurityDefinition` and `AddSecurityRequirement` are configured in Swagger setup
- Rebuild and refresh Swagger UI

---

## Next Steps

Once Phase 4 is complete and verified:

➡️ **Proceed to [Phase 5: Use Case Layer Updates](./KEYCLOAK_IMPLEMENTATION_PHASE_5.md)**

Phase 5 will update the use case handlers (Register, Login, RefreshToken) to integrate with Keycloak services instead of custom authentication logic.

---

_Phase 4 Last Updated: 2026-03-05_  
_Author: Development Team_
