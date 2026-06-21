using SimpleECommerceBackend.Application.Models.Auth;

namespace SimpleECommerceBackend.Api.Dtos.V1.Auth;

public class LoginRequest
{
    public string? ReturnUrl { get; init; }

    public static BeginLoginCommand ToCommand(LoginRequest request)
    {
        return new BeginLoginCommand
        {
            ReturnUrl = request.ReturnUrl
        };
    }
}
