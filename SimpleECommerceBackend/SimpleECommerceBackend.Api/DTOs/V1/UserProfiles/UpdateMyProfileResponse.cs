using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;

public class UpdateMyProfileResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public string Status { get; init; } = null!;
    public DateOnly BirthDate { get; init; }

    public static UpdateMyProfileResponse FromResult(UpdateMyProfileResult result)
    {
        return new UpdateMyProfileResponse
        {
            Id = result.Id,
            Email = result.Email,
            FirstName = result.FirstName,
            LastName = result.LastName,
            NickName = result.NickName,
            Sex = EnumUtils.ToDisplayValue(result.Sex),
            Status = EnumUtils.ToDisplayValue(result.Status),
            BirthDate = result.BirthDate
        };
    }
}