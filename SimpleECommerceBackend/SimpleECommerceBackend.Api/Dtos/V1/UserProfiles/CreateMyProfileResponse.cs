using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.UserProfiles;

public class CreateMyProfileResponse
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public DateOnly BirthDate { get; init; }

    public static CreateMyProfileResponse FromResult(CreateMyProfileResult result)
    {
        return new CreateMyProfileResponse
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
            NickName = result.NickName,
            Sex = EnumUtils.ToDisplayValue(result.Sex),
            BirthDate = result.BirthDate
        };
    }
}