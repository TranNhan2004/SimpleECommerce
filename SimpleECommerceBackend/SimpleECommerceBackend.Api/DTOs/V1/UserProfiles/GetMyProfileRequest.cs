using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;

public class GetMyProfileRequest
{
    public static GetMyProfileQuery ToQuery(GetMyProfileRequest request)
    {
        return new GetMyProfileQuery();
    }
}