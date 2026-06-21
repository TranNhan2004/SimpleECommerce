using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Users;

public class UpdateMeRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? NickName { get; init; }
    public string? Sex { get; init; }
    public DateOnly? BirthDate { get; init; }

    public static UpdateMeCommand ToCommand(UpdateMeRequest request)
    {
        return new UpdateMeCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            NickName = request.NickName,
            Sex = request.Sex is not null ? EnumUtils.FromDisplayValue<Sex>(request.Sex) : null,
            BirthDate = request.BirthDate
        };
    }
}
