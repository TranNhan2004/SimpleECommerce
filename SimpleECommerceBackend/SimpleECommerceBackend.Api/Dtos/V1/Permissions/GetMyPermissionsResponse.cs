using SimpleECommerceBackend.Application.Models.Permissions;

namespace SimpleECommerceBackend.Api.Dtos.V1.Permissions;

public class GetMyPermissionsResponse
{
    public IReadOnlyList<string> Permissions { get; init; } = [];

    public static GetMyPermissionsResponse FromResult(GetMyPermissionsResult result)
    {
        return new GetMyPermissionsResponse
        {
            Permissions = result.Permissions
        };
    }
}
