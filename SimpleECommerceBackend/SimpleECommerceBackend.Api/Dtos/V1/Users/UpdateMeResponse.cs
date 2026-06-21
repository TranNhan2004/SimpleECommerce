using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Users;

public class UpdateMeResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public string Status { get; init; } = null!;
    public DateOnly BirthDate { get; init; }

    public static UpdateMeResponse FromResult(UpdateMeResult result)
    {
        return new UpdateMeResponse
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
