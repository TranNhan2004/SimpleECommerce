using System.ComponentModel.DataAnnotations;
using SimpleECommerceBackend.Domain.Constants;

namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class LoginRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
