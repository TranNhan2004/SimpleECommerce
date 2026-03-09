using System.ComponentModel.DataAnnotations;

namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class ConfirmEmailRequest
{
    public string Token { get; init; } = null!;
}