using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleECommerceBackend.Api.DTOs.Auth;

namespace SimpleECommerceBackend.Api.Tests.Integration.Auth;

/// <summary>
/// Integration tests for authentication endpoints.
/// NOTE: These tests require a running Keycloak instance at http://localhost:8080
/// Run docker-compose up before executing these tests.
/// </summary>
public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // ---------- Register Endpoint Tests ----------

    [Fact]
    public async Task Register_Should_Return200_WithValidData()
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
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        result.Should().NotBeNull();
        result!.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Register_Should_Return409_WhenUserAlreadyExists()
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
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Theory]
    [InlineData("customer")]
    [InlineData("seller")]
    [InlineData("admin")]
    public async Task Register_Should_AcceptValidRoles(string role)
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = $"test_{role}_{Guid.NewGuid()}@example.com",
            Password = "Test@123",
            FirstName = "Test",
            LastName = "User",
            Role = role
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_Should_Return422_WhenRoleIsInvalid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = $"testuser_{Guid.NewGuid()}@example.com",
            Password = "Test@123",
            FirstName = "Test",
            LastName = "User",
            Role = "invalid_role"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ---------- Login Endpoint Tests ----------

    [Fact]
    public async Task Login_Should_Return200_WithValidCredentials()
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

        // Small delay to ensure Keycloak processes the registration
        await Task.Delay(500);

        // Act - Login
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.ExpiresIn.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Login_Should_Return401_WithInvalidPassword()
    {
        // Arrange - First register a user
        var email = $"logintest_{Guid.NewGuid()}@example.com";
        var correctPassword = "Test@123";

        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = correctPassword,
            FirstName = "Test",
            LastName = "User",
            Role = "customer"
        });

        await Task.Delay(500);

        // Act - Try to login with wrong password
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = "WrongPassword@123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Should_Return401_WithNonExistentUser()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Test@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Should_ReturnUserInfo_WithCorrectRole()
    {
        // Arrange - Register a seller
        var email = $"seller_{Guid.NewGuid()}@example.com";
        var password = "Test@123";

        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            FirstName = "Seller",
            LastName = "User",
            Role = "seller"
        });

        await Task.Delay(500);

        // Act - Login
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Role.Should().Be("seller");
    }

    // ---------- Refresh Token Endpoint Tests ----------

    [Fact]
    public async Task RefreshToken_Should_Return200_WithValidRefreshToken()
    {
        // Arrange - Register and login to get a refresh token
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

        await Task.Delay(500);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        var refreshToken = loginResult!.RefreshToken;

        // Act - Refresh the token
        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = refreshToken
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.ExpiresIn.Should().BeGreaterThan(0);

        // The new access token should be different from the original
        result.AccessToken.Should().NotBe(loginResult.AccessToken);
    }

    [Fact]
    public async Task RefreshToken_Should_Return401_WithInvalidRefreshToken()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = "invalid_refresh_token_12345"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_Should_Return401_WithEmptyRefreshToken()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ---------- End-to-End Flow Test ----------

    [Fact]
    public async Task CompleteAuthFlow_Should_Work_FromRegisterToRefresh()
    {
        // Arrange
        var email = $"fullflow_{Guid.NewGuid()}@example.com";
        var password = "Test@123";

        // Step 1: Register
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest
        {
            Email = email,
            Password = password,
            FirstName = "Full",
            LastName = "Flow",
            Role = "customer"
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        await Task.Delay(500);

        // Step 2: Login
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        // Step 3: Refresh token
        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh", new RefreshTokenRequest
        {
            RefreshToken = loginResult!.RefreshToken
        });

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<RefreshTokenResponse>();

        // Assert - All steps succeeded
        refreshResult.Should().NotBeNull();
        refreshResult!.AccessToken.Should().NotBe(loginResult.AccessToken);
    }
}
