using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.UserProfiles;

public class CreateMyProfileRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public DateOnly BirthDate { get; init; }

    public static CreateMyProfileCommand ToCommand(CreateMyProfileRequest request)
    {
        return new CreateMyProfileCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            NickName = request.NickName,
            Sex = EnumUtils.FromDisplayValue<Sex>(request.Sex),
            BirthDate = request.BirthDate
        };
    }

}