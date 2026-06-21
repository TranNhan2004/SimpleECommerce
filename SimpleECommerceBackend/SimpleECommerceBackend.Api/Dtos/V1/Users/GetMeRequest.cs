using SimpleECommerceBackend.Application.Models.Users;

namespace SimpleECommerceBackend.Api.Dtos.V1.Users;

public class GetMeRequest
{
    public static GetMeQuery ToQuery(GetMeRequest request)
    {
        return new GetMeQuery();
    }
}
