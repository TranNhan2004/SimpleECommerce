namespace SimpleECommerceBackend.Application.Models.Auth;

public class ConfirmEmailResult
{
    public string Email { get; init; } = null!;
    public string Message { get; init; } = null!;
    public DateTimeOffset ConfirmedAt { get; init; }
    public bool AlreadyConfirmed { get; init; }
}