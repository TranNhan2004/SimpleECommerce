using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;

public class UpdateMyProfileRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? NickName { get; init; }
    public string? Sex { get; init; }
    public DateOnly? BirthDate { get; init; }

    public static UpdateMyProfileCommand ToCommand(UpdateMyProfileRequest request)
    {
        return new UpdateMyProfileCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            NickName = request.NickName,
            Sex = request.Sex is not null ? EnumUtils.FromDisplayValue<Sex>(request.Sex) : null,
            BirthDate = request.BirthDate
        };
    }
}