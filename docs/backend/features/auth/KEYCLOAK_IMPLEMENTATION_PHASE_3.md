# Phase 3: Authentication Service Implementation

**Status**: ✅ Complete  
**Duration**: 1-2 days  
**Phase Overview**: [KEYCLOAK_IMPLEMENTATION_PLAN.md](./KEYCLOAK_IMPLEMENTATION_PLAN.md)

---

## Table of Contents

1. [Objectives](#objectives)
2. [Prerequisites](#prerequisites)
3. [Implementation Steps](#implementation-steps)
   - [Step 3.1: Create Keycloak Token Service Interface](#step-31-create-keycloak-token-service-interface)
   - [Step 3.2: Implement Keycloak Token Service](#step-32-implement-keycloak-token-service)
   - [Step 3.3: Create Keycloak Admin Service Interface](#step-33-create-keycloak-admin-service-interface)
   - [Step 3.4: Implement Keycloak Admin Service](#step-34-implement-keycloak-admin-service)
   - [Step 3.5: Register Services in Dependency Injection](#step-35-register-services-in-dependency-injection)
4. [Verification Checklist](#verification-checklist)
5. [Service Architecture Notes](#service-architecture-notes)
6. [Next Steps](#next-steps)

---

## Objectives

- ✅ Create IKeycloakTokenService interface for authentication operations
- ✅ Implement KeycloakTokenService for token management
- ✅ Create IKeycloakAdminService interface for user management
- ✅ Implement KeycloakAdminService for administrative operations
- ✅ Handle token validation and refresh logic
- ✅ Manage user creation, role assignment, and deletion in Keycloak

---

## Prerequisites

- [x] Phase 1 completed successfully (Keycloak setup)
  - Client: `simple-e-commerce-backend` with client secret
  - Realm: `SimpleECommerce` with roles: customer, seller, admin
  - Test users: admin@test.com, customer@test.com, seller@test.com
  - Token endpoint verified and working
- [x] Phase 2 completed successfully (Backend configuration)
- [x] Keycloak client secret configured in appsettings.json
- [x] Understanding of OAuth2 token flows
- [x] Familiarity with HttpClient usage in .NET

---

## Implementation Steps

### Step 3.1: Create Keycloak Token Service Interface

Create the interface that defines token management operations including authentication, token refresh, user information retrieval, and token validation.

**File**: `SimpleECommerceBackend.Application/Interfaces/Services/Keycloak/IKeycloakTokenService.cs`

```csharp
using SimpleECommerceBackend.Application.Models.Keycloak;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;

public interface IKeycloakTokenService
{
    Task<KeycloakTokenResponse> GetTokenAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<KeycloakTokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<KeycloakUserInfoResponse> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}
```

**DTOs**: Create separate model files in `SimpleECommerceBackend.Application/Models/Keycloak/`

**File**: `KeycloakTokenResponse.cs`

```csharp
namespace SimpleECommerceBackend.Application.Models.Keycloak;

public class KeycloakTokenResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
    public int RefreshExpiresIn { get; init; }
    public string TokenType { get; init; } = null!;
    public string Scope { get; init; } = null!;
}
```

**File**: `KeycloakUserInfoResponse.cs`

```csharp
namespace SimpleECommerceBackend.Application.Models.Keycloak;

public class KeycloakUserInfoResponse
{
    public string Sub { get; init; } = null!;
    public string Email { get; init; } = null!;
    public bool EmailVerified { get; init; }
    public string PreferredUsername { get; init; } = null!;
    public string GivenName { get; init; } = null!;
    public string FamilyName { get; init; } = null!;
    public List<string> Roles { get; init; } = [];
}
```

**📝 Interface Responsibilities:**

- **GetTokenAsync**: Authenticate user with username/password and obtain access + refresh tokens
- **RefreshTokenAsync**: Obtain new tokens using a valid refresh token
- **GetUserInfoAsync**: Retrieve user information from Keycloak using access token
- **ValidateTokenAsync**: Validate token using Keycloak's introspection endpoint

---

### Step 3.2: Implement Keycloak Token Service

Create the implementation that handles all token-related operations with Keycloak.

**File**: `SimpleECommerceBackend.Infrastructure/Services/Keycloak/KeycloakTokenService.cs`

```csharp
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Services.Keycloak;

public class KeycloakTokenService : IKeycloakTokenService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _settings;

    public KeycloakTokenService(HttpClient httpClient, IOptions<KeycloakSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<KeycloakTokenResponse> GetTokenAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint);

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", _settings.Resource },
            { "client_secret", _settings.Credentials.Secret },
            { "username", username },
            { "password", password },
            { "scope", "openid profile email simple-e-commerce-roles" }
        };

        request.Content = new FormUrlEncodedContent(parameters);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new UnauthorizedException($"Failed to authenticate: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (tokenResponse == null)
                throw new UnauthorizedException("Invalid token response from Keycloak");

            return new KeycloakTokenResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                RefreshExpiresIn = tokenResponse.RefreshExpiresIn,
                TokenType = tokenResponse.TokenType,
                Scope = tokenResponse.Scope
            };
        }
        catch (HttpRequestException ex)
        {
            throw new UnauthorizedException($"Failed to connect to Keycloak: {ex.Message}");
        }
    }

    public async Task<KeycloakTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint);

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", _settings.Resource },
            { "client_secret", _settings.Credentials.Secret },
            { "refresh_token", refreshToken }
        };

        request.Content = new FormUrlEncodedContent(parameters);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedException("Invalid or expired refresh token");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (tokenResponse == null)
                throw new UnauthorizedException("Invalid token response from Keycloak");

            return new KeycloakTokenResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                RefreshExpiresIn = tokenResponse.RefreshExpiresIn,
                TokenType = tokenResponse.TokenType,
                Scope = tokenResponse.Scope
            };
        }
        catch (HttpRequestException ex)
        {
            throw new UnauthorizedException($"Failed to refresh token: {ex.Message}");
        }
    }

    public async Task<KeycloakUserInfoResponse> GetUserInfoAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _settings.UserInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedException("Invalid access token");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var userInfo = JsonSerializer.Deserialize<KeycloakUserInfoDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (userInfo == null)
                throw new UnauthorizedException("Invalid user info response");

            return new KeycloakUserInfoResponse
            {
                Sub = userInfo.Sub,
                Email = userInfo.Email,
                EmailVerified = userInfo.EmailVerified,
                PreferredUsername = userInfo.PreferredUsername,
                GivenName = userInfo.GivenName ?? "",
                FamilyName = userInfo.FamilyName ?? "",
                Roles = userInfo.Roles ?? new List<string>()
            };
        }
        catch (HttpRequestException ex)
        {
            throw new UnauthorizedException($"Failed to get user info: {ex.Message}");
        }
    }

    public async Task<bool> ValidateTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.IntrospectionEndpoint);

        var parameters = new Dictionary<string, string>
        {
            { "client_id", _settings.Resource },
            { "client_secret", _settings.Credentials.Secret },
            { "token", accessToken }
        };

        request.Content = new FormUrlEncodedContent(parameters);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var introspection = JsonSerializer.Deserialize<TokenIntrospectionDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return introspection?.Active ?? false;
        }
        catch
        {
            return false;
        }
    }

    // DTOs for deserialization
    private class KeycloakTokenResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public int RefreshExpiresIn { get; set; }
        public string TokenType { get; set; } = null!;
        public string Scope { get; set; } = null!;
    }

    private class KeycloakUserInfoDto
    {
        public string Sub { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool EmailVerified { get; set; }
        public string PreferredUsername { get; set; } = null!;
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public List<string>? Roles { get; set; }
    }

    private class TokenIntrospectionDto
    {
        public bool Active { get; set; }
    }
}
```

**📝 Implementation Details:**

- Uses HttpClient for REST API calls to Keycloak
- Proper exception handling with domain-specific exceptions
- DTOs for JSON deserialization (private nested classes)
- OAuth2 standard grant types: `password` and `refresh_token`
- Token introspection for validation

---

### Step 3.3: Create Keycloak Admin Service Interface

Create the interface for administrative operations including user management, role assignment, and user queries.

**File**: `SimpleECommerceBackend.Application/Interfaces/Services/Keycloak/IKeycloakAdminService.cs`

```csharp
using SimpleECommerceBackend.Application.Models.Keycloak;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;

public interface IKeycloakAdminService
{
    Task<CreateKeycloakUserResponse> CreateUserAsync(CreateKeycloakUserRequest request, CancellationToken cancellationToken = default);
    Task AssignRoleToUserAsync(string userId, string roleName, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
}
```

**DTOs**: Create separate model files in `SimpleECommerceBackend.Application/Models/Keycloak/`

**File**: `CreateKeycloakUserRequest.cs`

```csharp
namespace SimpleECommerceBackend.Application.Models.Keycloak;

public class CreateKeycloakUserRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Role { get; init; } = null!;
}
```

**File**: `CreateKeycloakUserResponse.cs`

```csharp
namespace SimpleECommerceBackend.Application.Models.Keycloak;

public class CreateKeycloakUserResponse
{
    public string KeycloakUserId { get; init; } = null!;
    public string Email { get; init; } = null!;
}
```

**📝 Interface Responsibilities:**

- **CreateUserAsync**: Create a new user in Keycloak with credentials
- **AssignRoleToUserAsync**: Assign realm role to user
- **DeleteUserAsync**: Delete user from Keycloak
- **UserExistsAsync**: Check if user exists by email

---

### Step 3.4: Implement Keycloak Admin Service

Create the implementation that handles all administrative operations with Keycloak Admin REST API.

**File**: `SimpleECommerceBackend.Infrastructure/Services/Keycloak/KeycloakAdminService.cs`

```csharp
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Services.Keycloak;

public class KeycloakAdminService : IKeycloakAdminService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _settings;
    private string? _adminToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public KeycloakAdminService(HttpClient httpClient, IOptions<KeycloakSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<CreateKeycloakUserResponse> CreateUserAsync(
        CreateKeycloakUserRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        var usersUrl = $"{_settings.AdminUrl}/users";
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, usersUrl);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

        var userRepresentation = new
        {
            username = request.Email,
            email = request.Email,
            firstName = request.FirstName,
            lastName = request.LastName,
            enabled = true,
            emailVerified = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = request.Password,
                    temporary = false
                }
            }
        };

        var json = JsonSerializer.Serialize(userRepresentation);
        httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    throw new BusinessException("User with this email already exists");

                throw new BusinessException($"Failed to create user in Keycloak: {errorContent}");
            }

            // Get user ID from location header
            var locationHeader = response.Headers.Location?.ToString();
            if (string.IsNullOrEmpty(locationHeader))
                throw new BusinessException("Failed to retrieve user ID from Keycloak");

            var userId = locationHeader.Split('/').Last();

            // Assign role
            await AssignRoleToUserAsync(userId, request.Role, cancellationToken);

            return new CreateKeycloakUserResponse
            {
                KeycloakUserId = userId,
                Email = request.Email
            };
        }
        catch (HttpRequestException ex)
        {
            throw new BusinessException($"Failed to connect to Keycloak: {ex.Message}");
        }
    }

    public async Task AssignRoleToUserAsync(
        string userId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        // Get role by name
        var rolesUrl = $"{_settings.AdminUrl}/roles/{roleName}";
        var getRoleRequest = new HttpRequestMessage(HttpMethod.Get, rolesUrl);
        getRoleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

        var roleResponse = await _httpClient.SendAsync(getRoleRequest, cancellationToken);
        if (!roleResponse.IsSuccessStatusCode)
            throw new BusinessException($"Role '{roleName}' not found in Keycloak");

        var roleContent = await roleResponse.Content.ReadAsStringAsync(cancellationToken);
        var role = JsonSerializer.Deserialize<KeycloakRoleDto>(roleContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (role == null)
            throw new BusinessException($"Failed to deserialize role '{roleName}'");

        // Assign role to user
        var assignRoleUrl = $"{_settings.AdminUrl}/users/{userId}/role-mappings/realm";
        var assignRequest = new HttpRequestMessage(HttpMethod.Post, assignRoleUrl);
        assignRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

        var roleArray = new[] { new { id = role.Id, name = role.Name } };
        var json = JsonSerializer.Serialize(roleArray);
        assignRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var assignResponse = await _httpClient.SendAsync(assignRequest, cancellationToken);
        if (!assignResponse.IsSuccessStatusCode)
        {
            var errorContent = await assignResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new BusinessException($"Failed to assign role: {errorContent}");
        }
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        var deleteUrl = $"{_settings.AdminUrl}/users/{userId}";
        var request = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new BusinessException("Failed to delete user from Keycloak");
        }
    }

    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        await EnsureAdminTokenAsync(cancellationToken);

        var searchUrl = $"{_settings.AdminUrl}/users?email={Uri.EscapeDataString(email)}&exact=true";
        var request = new HttpRequestMessage(HttpMethod.Get, searchUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var users = JsonSerializer.Deserialize<List<object>>(content);

        return users != null && users.Count > 0;
    }

    private async Task EnsureAdminTokenAsync(CancellationToken cancellationToken)
    {
        if (_adminToken != null && DateTime.UtcNow < _tokenExpiration)
            return;

        // Get admin token using service account (client credentials)
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint);

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _settings.Resource },
            { "client_secret", _settings.Credentials.Secret }
        };

        request.Content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new BusinessException("Failed to obtain admin token from Keycloak");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<AdminTokenResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (tokenResponse == null)
            throw new BusinessException("Invalid admin token response");

        _adminToken = tokenResponse.AccessToken;
        _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Refresh 1 min before expiry
    }

    private class AdminTokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public int ExpiresIn { get; set; }
    }

    private class KeycloakRoleDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
```

**📝 Implementation Details:**

- **Admin Token Management**: Automatically obtains and caches admin token using client credentials grant
- **Token Refresh**: Expires token 1 minute before actual expiry to prevent race conditions
- **User Creation**: Creates user with credentials and assigns role in a single operation
- **Role Assignment**: Retrieves role by name, then assigns to user using role-mappings API
- **Error Handling**: Specific exceptions for different failure scenarios (conflict, not found, etc.)

---

### Step 3.5: Register Services in Dependency Injection

Now that we have created the services, we need to register them in the DI container. This is done in the Infrastructure layer's `DependencyInjection.cs` file.

**File**: `SimpleECommerceBackend.Infrastructure/DependencyInjection.cs`

**Add the following registration code after the existing Auth Services section:**

```csharp
// Keycloak Services
services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak"));

services.AddHttpClient<IKeycloakTokenService, KeycloakTokenService>((sp, client) =>
{
    var keycloakSettings = configuration.GetSection("Keycloak").Get<KeycloakSettings>();
    client.BaseAddress = new Uri(keycloakSettings!.AuthServerUrl);
    client.Timeout = TimeSpan.FromSeconds(keycloakSettings!.TimeoutSeconds);
});

services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>((sp, client) =>
{
    var keycloakSettings = configuration.GetSection("Keycloak").Get<KeycloakSettings>();
    client.BaseAddress = new Uri(keycloakSettings!.AuthServerUrl);
    client.Timeout = TimeSpan.FromSeconds(keycloakSettings!.TimeoutSeconds);
});
```

**📝 Implementation Details:**

- **AddHttpClient<TInterface, TImplementation>**: Registers both the service and a configured HttpClient
- **HttpClient Configuration**: Sets base address to Keycloak server URL from configuration
- **Dynamic Timeout**: Uses `TimeoutSeconds` from settings for configurable HTTP request timeout
- **Options Pattern**: KeycloakSettings are bound from configuration using IOptions pattern
- **HttpClient Configuration**: Sets base address to Keycloak server URL from configuration
- **Timeout**: Sets 30-second timeout for all HTTP requests
- **Options Pattern**: KeycloakSettings are bound from configuration using IOptions pattern

---

## Verification Checklist

Use this checklist to verify that Phase 3 has been completed successfully:

### Code Files Created

- [x] `SimpleECommerceBackend.Application/Interfaces/Services/Keycloak/IKeycloakTokenService.cs` exists
- [x] `SimpleECommerceBackend.Infrastructure/Services/Keycloak/KeycloakTokenService.cs` exists
- [x] `SimpleECommerceBackend.Application/Interfaces/Services/Keycloak/IKeycloakAdminService.cs` exists
- [x] `SimpleECommerceBackend.Infrastructure/Services/Keycloak/KeycloakAdminService.cs` exists
- [x] `SimpleECommerceBackend.Application/Models/Keycloak/KeycloakTokenResponse.cs` exists
- [x] `SimpleECommerceBackend.Application/Models/Keycloak/KeycloakUserInfoResponse.cs` exists
- [x] `SimpleECommerceBackend.Application/Models/Keycloak/CreateKeycloakUserRequest.cs` exists
- [x] `SimpleECommerceBackend.Application/Models/Keycloak/CreateKeycloakUserResponse.cs` exists
- [x] `SimpleECommerceBackend.Infrastructure/Services/Keycloak/KeycloakSettings.cs` exists

### Code Quality

- [x] All files compile without errors
- [x] No unused namespaces or imports
- [x] Proper exception handling in all methods
- [x] All async methods accept CancellationToken parameter
- [x] DTOs are properly defined for JSON deserialization

### Service Implementation

- [x] KeycloakTokenService implements all IKeycloakTokenService methods
- [x] KeycloakAdminService implements all IKeycloakAdminService methods
- [x] Admin token caching logic is implemented correctly
- [x] Token expiration handling is in place

### Dependency Injection

- [x] KeycloakSettings configured in DependencyInjection.cs
- [x] IKeycloakTokenService registered with HttpClient
- [x] IKeycloakAdminService registered with HttpClient
- [x] HttpClient configured with base address and timeout

### Build Verification

**Test Build**:

```bash
cd SimpleECommerceBackend
dotnet build
```

Expected: Build succeeds with no errors.

**Check Keycloak Connection** (optional manual test):

You can test the services later in Phase 4 when they are registered in DI container.

**🎉 Phase 3 Complete**: If all checkboxes are checked, you're ready for Phase 4!

---

## Service Architecture Notes

### Token Service Design

The `KeycloakTokenService` follows these design principles:

1. **Separation of Concerns**: Token operations are isolated from user management
2. **Dependency Injection**: Uses IOptions pattern for configuration
3. **Error Handling**: Converts HTTP errors to domain exceptions
4. **Async/Await**: All methods are async for better performance

### Admin Service Design

The `KeycloakAdminService` implements these patterns:

1. **Token Caching**: Admin tokens are cached and reused until near expiration
2. **Service Account**: Uses client credentials grant for admin operations
3. **Atomic Operations**: User creation and role assignment happen together
4. **Fail-Safe**: Conservative token expiration (1 minute buffer)

### Security Considerations

- **Client Secret**: Never log or expose the client secret
- **Admin Token**: Stored in memory only, never persisted
- **Error Messages**: Don't expose sensitive information in exceptions
- **HTTPS**: In production, always use HTTPS for Keycloak communication

### Performance Notes

- **HttpClient**: Should be registered as named/typed client in DI for connection pooling
- **Token Caching**: Reduces API calls to Keycloak
- **Parallel Operations**: Consider parallelizing independent operations in use cases

---

## Next Steps

After completing Phase 3, proceed to:

**[Phase 4: API Layer Updates](./KEYCLOAK_IMPLEMENTATION_PHASE_4.md)**

In Phase 4, you will:

- Update Program.cs with Keycloak authentication
- Register services in DI container
- Update AuthController endpoints
- Configure Swagger with JWT authentication
- Add authorization policies

---

_Phase 3 Last Updated: 2026-03-05_  
_Author: Development Team_
