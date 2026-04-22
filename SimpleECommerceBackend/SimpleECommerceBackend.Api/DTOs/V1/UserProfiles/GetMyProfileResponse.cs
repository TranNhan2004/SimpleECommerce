using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;

public class GetMyProfileResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public string Status { get; init; } = null!;
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }

    public static GetMyProfileResponse FromResult(GetMyProfileResult result)
    {
        return new GetMyProfileResponse
        {
            Id = result.Id,
            Email = result.Email,
            FirstName = result.FirstName,
            LastName = result.LastName,
            NickName = result.NickName,
            Sex = EnumUtils.ToDisplayValue(result.Sex),
            Status = EnumUtils.ToDisplayValue(result.Status),
            BirthDate = result.BirthDate,
            AvatarUrl = result.AvatarUrl
        };
    }
}