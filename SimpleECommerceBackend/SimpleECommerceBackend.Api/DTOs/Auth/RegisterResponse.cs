namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class RegisterResponse
{
    public string Email { get; init; } = null!;
    public string Message { get; init; } = "User registered successfully";
}