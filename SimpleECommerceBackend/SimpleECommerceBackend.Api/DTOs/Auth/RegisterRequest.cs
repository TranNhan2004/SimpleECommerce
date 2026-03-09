using System.ComponentModel.DataAnnotations;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class RegisterRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;

    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Role { get; init; } = RoleUtils.ToKeycloakRoleName(RoleUtils.DefaultRole);
}
