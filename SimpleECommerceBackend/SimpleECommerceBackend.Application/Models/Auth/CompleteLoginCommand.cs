namespace SimpleECommerceBackend.Application.Models.Auth;

public class CompleteLoginCommand
{
    public string? Code { get; init; }
    public string? State { get; init; }
    public string? Error { get; init; }
    public string? ErrorDescription { get; init; }
}
