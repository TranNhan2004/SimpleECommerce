using SimpleECommerceBackend.Application.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace SimpleECommerceBackend.Api.Dtos.V1.Auth;

public class LoginCallbackRequest
{
    public string? Code { get; init; }
    public string? State { get; init; }
    public string? Error { get; init; }

    [FromQuery(Name = "error_description")]
    public string? ErrorDescription { get; init; }

    public static CompleteLoginCommand ToCommand(LoginCallbackRequest request)
    {
        return new CompleteLoginCommand
        {
            Code = request.Code,
            State = request.State,
            Error = request.Error,
            ErrorDescription = request.ErrorDescription
        };
    }
}
