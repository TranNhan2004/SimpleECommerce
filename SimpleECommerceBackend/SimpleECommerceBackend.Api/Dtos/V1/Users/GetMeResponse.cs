using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Users;

public class GetMeResponse
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
    public IReadOnlyList<string> Permissions { get; init; } = [];

    public static GetMeResponse FromResult(GetMeResult result)
    {
        return new GetMeResponse
        {
            Id = result.Id,
            Email = result.Email,
            FirstName = result.FirstName,
            LastName = result.LastName,
            NickName = result.NickName,
            Sex = EnumUtils.ToDisplayValue(result.Sex),
            Status = EnumUtils.ToDisplayValue(result.Status),
            BirthDate = result.BirthDate,
            AvatarUrl = result.AvatarUrl,
            Permissions = result.Permissions
        };
    }
}
