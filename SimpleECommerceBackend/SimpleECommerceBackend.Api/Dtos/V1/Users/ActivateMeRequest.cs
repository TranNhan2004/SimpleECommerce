using SimpleECommerceBackend.Application.Models.Users;

namespace SimpleECommerceBackend.Api.Dtos.V1.Users;

public class ActivateMeRequest
{
    public static ActivateMeCommand ToCommand(ActivateMeRequest request)
    {
        return new ActivateMeCommand();
    }
}
