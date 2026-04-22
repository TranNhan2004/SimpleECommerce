using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;

public class DeleteMyProfileRequest
{
    public static DeleteMyProfileCommand ToCommand(DeleteMyProfileRequest request)
    {
        return new DeleteMyProfileCommand();
    }
}