# Phase 7: Testing & Validation

**Status**: ⬜ Not Started  
**Duration**: 2-3 days  
**Phase Overview**: [KEYCLOAK_IMPLEMENTATION_PLAN.md](./KEYCLOAK_IMPLEMENTATION_PLAN.md)

---

## Table of Contents

1. [Objectives](#objectives)
2. [Prerequisites](#prerequisites)
3. [Implementation Steps](#implementation-steps)
   - [Step 7.1: Create Keycloak Mocks for Unit Tests](#step-71-create-keycloak-mocks-for-unit-tests)
   - [Step 7.2: Update Unit Tests](#step-72-update-unit-tests)
   - [Step 7.3: Create Integration Tests](#step-73-create-integration-tests)
   - [Step 7.4: Manual API Testing](#step-74-manual-api-testing)
   - [Step 7.5: Test Authorization Policies](#step-75-test-authorization-policies)
4. [Verification Checklist](#verification-checklist)
5. [Troubleshooting](#troubleshooting)
6. [Next Steps](#next-steps)

---

## Objectives

- ✅ Create mock implementations of Keycloak services for testing
- ✅ Update existing unit tests to use Keycloak mocks
- ✅ Create integration tests for authentication flows
- ✅ Perform manual API testing with Postman/curl
- ✅ Validate role-based authorization policies
- ✅ Test token refresh mechanism
- ✅ Verify error handling and edge cases

---

## Prerequisites

- [x] Phase 1 completed (Keycloak Setup)
  - Test users available: admin@test.com, customer@test.com, seller@test.com
  - Passwords: Admin@123, Customer@123, Seller@123
  - Roles properly assigned in Keycloak
- [ ] Phase 2-6 completed
- [ ] Application builds successfully
- [ ] Keycloak instance running on http://localhost:8080
- [ ] Postman or equivalent API testing tool installed
- [ ] Understanding of xUnit and Moq

---

## Implementation Steps

### Step 7.1: Create Keycloak Mocks for Unit Tests

#### 7.1.1: Create MockKeycloakTokenService

**File**: `SimpleECommerceBackend.UnitTests/Mocks/MockKeycloakTokenService.cs`

Create the directory if it doesn't exist:

```bash
mkdir -p SimpleECommerceBackend.UnitTests/Mocks
```

Create the mock implementation:

```csharp
using SimpleECommerceBackend.Application.Interfaces.Security;

namespace SimpleECommerceBackend.UnitTests.Mocks;

public class MockKeycloakTokenService : IKeycloakTokenService
{
    private readonly Dictionary<string, string> _validCredentials = new()
    {
        { "test@example.com", "password123" },
        { "admin@example.com", "admin123" },
        { "customer@example.com", "customer123" }
    };

    private readonly Dictionary<string, KeycloakUserInfoResponse> _userInfoMap = new()
    {
        {
            "mock_access_token_test",
            new KeycloakUserInfoResponse
            {
                Sub = "550e8400-e29b-41d4-a716-446655440000",
                Email = "test@example.com",
                EmailVerified = true,
                PreferredUsername = "test@example.com",
                GivenName = "Test",
                FamilyName = "User",
                Roles = new List<string> { "customer" }
            }
        },
        {
            "mock_access_token_admin",
            new KeycloakUserInfoResponse
            {
                Sub = "550e8400-e29b-41d4-a716-446655440001",
                Email = "admin@example.com",
                EmailVerified = true,
                PreferredUsername = "admin@example.com",
                GivenName = "Admin",
                FamilyName = "User",
                Roles = new List<string> { "admin" }
            }
        }
    };

    public Task<KeycloakTokenResponse> GetTokenAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (!_validCredentials.TryGetValue(username, out var validPassword) || validPassword != password)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var tokenKey = username.Contains("admin") ? "mock_access_token_admin" : "mock_access_token_test";

        return Task.FromResult(new KeycloakTokenResponse
        {
            AccessToken = tokenKey,
            RefreshToken = $"mock_refresh_token_{Guid.NewGuid()}",
            ExpiresIn = 300,
            RefreshExpiresIn = 1800,
            TokenType = "Bearer",
            Scope = "openid profile email simple-e-commerce-roles"
        });
    }

    public Task<KeycloakTokenResponse> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (!refreshToken.StartsWith("mock_refresh_token_"))
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        return Task.FromResult(new KeycloakTokenResponse
        {
            AccessToken = $"mock_refreshed_access_token_{Guid.NewGuid()}",
            RefreshToken = $"mock_new_refresh_token_{Guid.NewGuid()}",
            ExpiresIn = 300,
            RefreshExpiresIn = 1800,
            TokenType = "Bearer",
            Scope = "openid profile email simple-e-commerce-roles"
        });
    }

    public Task<KeycloakUserInfoResponse> GetUserInfoAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (_userInfoMap.TryGetValue(accessToken, out var userInfo))
        {
            return Task.FromResult(userInfo);
        }

        // Default user info for unknown tokens
        return Task.FromResult(new KeycloakUserInfo
        {
            Sub = Guid.NewGuid().ToString(),
            Email = "unknown@example.com",
            EmailVerified = true,
            PreferredUsername = "unknown@example.com",
            GivenName = "Unknown",
            FamilyName = "User",
            Roles = new List<string> { "customer" }
        });
    }

    public Task<bool> ValidateTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        // Simple validation: check if token starts with expected prefix
        var isValid = accessToken.StartsWith("mock_access_token_") ||
                      accessToken.StartsWith("mock_refreshed_access_token_");
        return Task.FromResult(isValid);
    }
}
```

#### 7.1.2: Create MockKeycloakAdminService

**File**: `SimpleECommerceBackend.UnitTests/Mocks/MockKeycloakAdminService.cs`

```csharp
using SimpleECommerceBackend.Application.Interfaces.Security;

namespace SimpleECommerceBackend.UnitTests.Mocks;

public class MockKeycloakAdminService : IKeycloakAdminService
{
    private readonly HashSet<string> _existingEmails = new()
    {
        "existing@example.com",
        "admin@example.com"
    };

    private readonly Dictionary<string, CreateKeycloakUserResponse> _createdUsers = new();

    public Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = _existingEmails.Contains(email);
        return Task.FromResult(exists);
    }

    public Task<CreateKeycloakUserResponse> CreateUserAsync(
        CreateKeycloakUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_existingEmails.Contains(request.Email))
        {
            throw new InvalidOperationException("User already exists");
        }

        var userId = Guid.NewGuid().ToString();
        var response = new CreateKeycloakUserResponse
        {
            KeycloakUserId = userId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _createdUsers[request.Email] = response;
        _existingEmails.Add(request.Email);

        return Task.FromResult(response);
    }

    public Task<KeycloakUser> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = new KeycloakUser
        {
            Id = userId,
            Email = "user@example.com",
            FirstName = "Test",
            LastName = "User",
            EmailVerified = true,
            Enabled = true
        };

        return Task.FromResult(user);
    }

    public Task UpdateUserAsync(
        string userId,
        UpdateKeycloakUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Mock implementation - just succeed
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Mock implementation - just succeed
        return Task.CompletedTask;
    }

    public Task AssignRoleAsync(
        string userId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        // Mock implementation - just succeed
        return Task.CompletedTask;
    }

    public Task RemoveRoleAsync(
        string userId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        // Mock implementation - just succeed
        return Task.CompletedTask;
    }

    public Task ResetPasswordAsync(
        string userId,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        // Mock implementation - just succeed
        return Task.CompletedTask;
    }

    public Task SendVerificationEmailAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        // Mock implementation - just succeed
        return Task.CompletedTask;
    }
}
```

---

### Step 7.2: Update Unit Tests

#### 7.2.1: Update RegisterCommandHandler Tests

**File**: `SimpleECommerceBackend.UnitTests/Application/Auth/RegisterCommandHandlerTests.cs`

```csharp
using SimpleECommerceBackend.Application.UseCases.Auth.Register;
using SimpleECommerceBackend.UnitTests.Mocks;
using Xunit;

namespace SimpleECommerceBackend.UnitTests.Application.Auth;

public class RegisterCommandHandlerTests
{
    private readonly MockKeycloakAdminService _mockKeycloakAdminService;
    private readonly MockUserProfileRepository _mockUserProfileRepository;
    private readonly MockUnitOfWork _mockUnitOfWork;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _mockKeycloakAdminService = new MockKeycloakAdminService();
        _mockUserProfileRepository = new MockUserProfileRepository();
        _mockUnitOfWork = new MockUnitOfWork();

        _handler = new RegisterCommandHandler(
            _mockKeycloakAdminService,
            _mockUserProfileRepository,
            _mockUnitOfWork
        );
    }

    [Fact]
    public async Task Register_Should_Create_User_In_Keycloak_And_LocalDB()
    {
        // Arrange
        var command = new RegisterCommand(
            "newuser@test.com",
            "Password@123",
            "New",
            "User",
            "customer"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser@test.com", result.Email);

        // Verify user exists in Keycloak
        var userExists = await _mockKeycloakAdminService.UserExistsAsync("newuser@test.com");
        Assert.True(userExists);

        // Verify SaveChanges was called
        Assert.True(_mockUnitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Register_Should_Throw_When_User_Already_Exists()
    {
        // Arrange
        var command = new RegisterCommand(
            "existing@example.com", // This email exists in mock
            "Password@123",
            "Existing",
            "User",
            "customer"
        );

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Register_Should_Throw_When_Role_Invalid()
    {
        // Arrange
        var command = new RegisterCommand(
            "newuser@test.com",
            "Password@123",
            "New",
            "User",
            "invalid_role" // Invalid role
        );

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
    }
}
```

#### 7.2.2: Update LoginCommandHandler Tests

**File**: `SimpleECommerceBackend.UnitTests/Application/Auth/LoginCommandHandlerTests.cs`

```csharp
using SimpleECommerceBackend.Application.UseCases.Auth.Login;
using SimpleECommerceBackend.UnitTests.Mocks;
using Xunit;

namespace SimpleECommerceBackend.UnitTests.Application.Auth;

public class LoginCommandHandlerTests
{
    private readonly MockKeycloakTokenService _mockTokenService;
    private readonly MockUserProfileRepository _mockUserProfileRepository;
    private readonly MockUnitOfWork _mockUnitOfWork;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockTokenService = new MockKeycloakTokenService();
        _mockUserProfileRepository = new MockUserProfileRepository();
        _mockUnitOfWork = new MockUnitOfWork();

        _handler = new LoginCommandHandler(
            _mockTokenService,
            _mockUserProfileRepository,
            _mockUnitOfWork
        );
    }

    [Fact]
    public async Task Login_Should_Return_Token_When_Credentials_Valid()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.True(result.ExpiresIn > 0);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task Login_Should_Throw_When_Credentials_Invalid()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "wrong_password");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Login_Should_Create_UserProfile_If_Not_Exists()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Verify that SaveChanges was called if profile was created
    }
}
```

#### 7.2.3: Update RefreshTokenCommandHandler Tests

**File**: `SimpleECommerceBackend.UnitTests/Application/Auth/RefreshTokenCommandHandlerTests.cs`

```csharp
using SimpleECommerceBackend.Application.UseCases.Auth.RefreshToken;
using SimpleECommerceBackend.UnitTests.Mocks;
using Xunit;

namespace SimpleECommerceBackend.UnitTests.Application.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly MockKeycloakTokenService _mockTokenService;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _mockTokenService = new MockKeycloakTokenService();
        _handler = new RefreshTokenCommandHandler(_mockTokenService);
    }

    [Fact]
    public async Task RefreshToken_Should_Return_New_Access_Token()
    {
        // Arrange
        var command = new RefreshTokenCommand("mock_refresh_token_12345");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.True(result.ExpiresIn > 0);
    }

    [Fact]
    public async Task RefreshToken_Should_Throw_When_Token_Invalid()
    {
        // Arrange
        var command = new RefreshTokenCommand("invalid_refresh_token");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
    }
}
```

---

### Step 7.3: Create Integration Tests

#### 7.3.1: Setup Integration Test Infrastructure

**File**: `SimpleECommerceBackend.IntegrationTest/Auth/AuthIntegrationTests.cs`

```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleECommerceBackend.Api.DTOs.Auth;
using Xunit;

namespace SimpleECommerceBackend.IntegrationTest.Auth;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Should_Return_200_With_Valid_Data()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = $"testuser_{Guid.NewGuid()}@example.com",
            Password = "Test@123",
            FirstName = "Test",
            LastName = "User",
            Role = "customer"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
    }

    [Fact]
    public async Task Register_Should_Return_409_When_User_Exists()
    {
        // Arrange
        var email = $"duplicate_{Guid.NewGuid()}@example.com";

        var request1 = new RegisterRequest
        {
            Email = email,
            Password = "Test@123",
            FirstName = "Test",
            LastName = "User",
            Role = "customer"
        };

        // Register first time
        await _client.PostAsJsonAsync("/api/v1/auth/register", request1);

        // Act - Try to register again with same email
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request1);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_Should_Return_200_With_Valid_Credentials()
    {
        // Arrange - First register a user
        var email = $"logintest_{Guid.NewGuid()}@example.com";
        var password = "Test@123";

        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User",
            Role = "customer"
        });

        // Act - Login
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task Login_Should_Return_401_With_Invalid_Credentials()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_Should_Return_New_Token()
    {
        // Arrange - Login first to get tokens
        var email = $"refreshtest_{Guid.NewGuid()}@example.com";
        var password = "Test@123";

        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User",
            Role = "customer"
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        // Act - Refresh token
        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = loginResult!.RefreshToken
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.NotEqual(loginResult.AccessToken, result.AccessToken);
    }
}
```

---

### Step 7.4: Manual API Testing

#### 7.4.1: Test with Postman

Create a Postman collection with the following requests:

**1. Register User (Customer)**

```http
POST http://localhost:5000/api/v1/auth/register
Content-Type: application/json

{
  "email": "customer@test.com",
  "password": "Customer@123",
  "firstName": "John",
  "lastName": "Doe",
  "role": "customer"
}
```

**Expected Response (200 OK):**

```json
{
  "email": "customer@test.com",
  "message": "User registered successfully"
}
```

**2. Register User (Seller)**

```http
POST http://localhost:5000/api/v1/auth/register
Content-Type: application/json

{
  "email": "seller@test.com",
  "password": "Seller@123",
  "firstName": "Jane",
  "lastName": "Smith",
  "role": "seller"
}
```

**3. Register User (Admin)**

```http
POST http://localhost:5000/api/v1/auth/register
Content-Type: application/json

{
  "email": "admin@test.com",
  "password": "Admin@123",
  "firstName": "Admin",
  "lastName": "User",
  "role": "admin"
}
```

**4. Login**

```http
POST http://localhost:5000/api/v1/auth/login
Content-Type: application/json

{
  "email": "customer@test.com",
  "password": "Customer@123"
}
```

**Expected Response (200 OK):**

```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "email": "customer@test.com",
  "firstName": "John",
  "lastName": "Doe",
  "nickName": null,
  "sex": "Other",
  "birthDate": "1998-05-15",
  "avatarUrl": null,
  "role": "customer",
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 300
}
```

**5. Refresh Token**

```http
POST http://localhost:5000/api/v1/auth/refresh
Content-Type: application/json

{
  "refreshToken": "{{refreshToken}}"
}
```

**6. Access Protected Endpoint**

```http
GET http://localhost:5000/api/v1/products
Authorization: Bearer {{accessToken}}
```

#### 7.4.2: Test with cURL

**Register:**

```bash
curl -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "curl@test.com",
    "password": "Test@123",
    "firstName": "Curl",
    "lastName": "Test",
    "role": "customer"
  }'
```

**Login:**

```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "curl@test.com",
    "password": "Test@123"
  }'
```

**Access Protected Endpoint:**

```bash
curl -X GET http://localhost:5000/api/v1/products \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

### Step 7.5: Test Authorization Policies

#### 7.5.1: Add Role-Based Authorization to Test Controller

**File**: `SimpleECommerceBackend.Api/Controllers/TestAuthController.cs` (Create for testing)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleECommerceBackend.Api.Controllers;

[Route("api/v{version:apiVersion}/test-auth")]
[ApiVersion("1.0")]
[ApiController]
public class TestAuthController : ControllerBase
{
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult PublicEndpoint()
    {
        return Ok(new { message = "This is a public endpoint" });
    }

    [HttpGet("customer")]
    [Authorize(Policy = "RequireCustomerRole")]
    public IActionResult CustomerEndpoint()
    {
        var email = User.Identity?.Name;
        return Ok(new { message = "This is a customer endpoint", user = email });
    }

    [HttpGet("seller")]
    [Authorize(Policy = "RequireSellerRole")]
    public IActionResult SellerEndpoint()
    {
        var email = User.Identity?.Name;
        return Ok(new { message = "This is a seller endpoint", user = email });
    }

    [HttpGet("admin")]
    [Authorize(Policy = "RequireAdminRole")]
    public IActionResult AdminEndpoint()
    {
        var email = User.Identity?.Name;
        return Ok(new { message = "This is an admin endpoint", user = email });
    }
}
```

#### 7.5.2: Test Each Role

**Test Matrix:**

| Endpoint            | Customer Token | Seller Token | Admin Token | No Token |
| ------------------- | -------------- | ------------ | ----------- | -------- |
| /test-auth/public   | ✅ 200         | ✅ 200       | ✅ 200      | ✅ 200   |
| /test-auth/customer | ✅ 200         | ❌ 403       | ❌ 403      | ❌ 401   |
| /test-auth/seller   | ❌ 403         | ✅ 200       | ❌ 403      | ❌ 401   |
| /test-auth/admin    | ❌ 403         | ❌ 403       | ✅ 200      | ❌ 401   |

**Test Customer Access:**

```bash
# Login as customer
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "customer@test.com", "password": "Customer@123"}'

# Test customer endpoint (should succeed)
curl -X GET http://localhost:5000/api/v1/test-auth/customer \
  -H "Authorization: Bearer CUSTOMER_TOKEN"

# Test seller endpoint (should fail with 403)
curl -X GET http://localhost:5000/api/v1/test-auth/seller \
  -H "Authorization: Bearer CUSTOMER_TOKEN"
```

---

## Verification Checklist

After completing this phase, verify the following:

### Unit Tests

- [ ] All unit tests pass
- [ ] Mock services work correctly
- [ ] RegisterCommandHandler tests updated
- [ ] LoginCommandHandler tests updated
- [ ] RefreshTokenCommandHandler tests updated
- [ ] Edge cases covered (invalid credentials, duplicate users, etc.)

### Integration Tests

- [ ] Integration tests pass
- [ ] Register endpoint creates users successfully
- [ ] Login endpoint returns valid tokens
- [ ] Refresh token endpoint works correctly
- [ ] Error responses match expected status codes

### Manual Testing

- [ ] Can register new users via API
- [ ] Can login with valid credentials
- [ ] Cannot login with invalid credentials
- [ ] Can refresh access token
- [ ] Tokens are valid JWT format
- [ ] Token contains correct claims (sub, email, roles)

### Authorization Testing

- [ ] Public endpoints accessible without token
- [ ] Protected endpoints require valid token
- [ ] Customer role can access customer endpoints only
- [ ] Seller role can access seller endpoints only
- [ ] Admin role can access admin endpoints only
- [ ] Invalid token returns 401 Unauthorized
- [ ] Valid token with wrong role returns 403 Forbidden

---

## Troubleshooting

### Issue: "Unit tests fail with DI errors"

**Solution:**

- Ensure all mock services implement required interfaces correctly
- Verify constructor parameters match handler requirements
- Check that mock methods return expected types

### Issue: "Integration tests timeout"

**Solution:**

- Verify Keycloak container is running
- Check network connectivity between test and Keycloak
- Increase timeout values in test configuration

### Issue: "Token validation fails in tests"

**Solution:**

- Ensure Keycloak configuration matches test environment
- Verify JWT signature validation is disabled for development
- Check that realm and client names match

### Issue: "Authorization policies don't work"

**Solution:**

- Verify roles are assigned to users in Keycloak
- Check that role claim is included in token (use jwt.io)
- Ensure policy names match role names exactly
- Verify `UseAuthentication()` is before `UseAuthorization()` in Program.cs

### Issue: "403 Forbidden on all endpoints"

**Solution:**

- Check token is being sent correctly in Authorization header
- Verify token hasn't expired
- Ensure user has required role assigned in Keycloak
- Check policy configuration in Program.cs

---

## Next Steps

Once Phase 7 is complete and all tests pass:

➡️ **Proceed to [Phase 8: Deployment & Migration](./KEYCLOAK_IMPLEMENTATION_PHASE_8.md)**

Phase 8 will cover production Keycloak setup, environment configuration, user migration strategies, and deployment procedures.
