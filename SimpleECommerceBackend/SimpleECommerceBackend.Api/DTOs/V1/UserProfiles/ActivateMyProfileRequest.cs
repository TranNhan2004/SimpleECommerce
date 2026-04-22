using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;

public class ActivateMyProfileRequest
{
    public static ActivateMyProfileCommand ToCommand(ActivateMyProfileRequest request)
    {
        return new ActivateMyProfileCommand();
    }
}