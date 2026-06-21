using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Users;

public class CreateMeRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public DateOnly BirthDate { get; init; }

    public static CreateMeCommand ToCommand(CreateMeRequest request)
    {
        return new CreateMeCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            NickName = request.NickName,
            Sex = EnumUtils.FromDisplayValue<Sex>(request.Sex),
            BirthDate = request.BirthDate
        };
    }
}
