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
        CancellationToken cancellationToken = default
    )
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
