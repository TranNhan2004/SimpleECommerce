using SimpleECommerceBackend.Application.Models.Users;

namespace SimpleECommerceBackend.Api.Dtos.V1.Users;

public class DeleteMeRequest
{
    public static DeleteMeCommand ToCommand(DeleteMeRequest request)
    {
        return new DeleteMeCommand();
    }
}
