using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Users;

public class CreateMeResponse
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public DateOnly BirthDate { get; init; }

    public static CreateMeResponse FromResult(CreateMeResult result)
    {
        return new CreateMeResponse
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
            NickName = result.NickName,
            Sex = EnumUtils.ToDisplayValue(result.Sex),
            BirthDate = result.BirthDate
        };
    }
}
