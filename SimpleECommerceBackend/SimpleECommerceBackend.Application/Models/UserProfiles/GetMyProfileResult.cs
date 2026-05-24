using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public class GetMyProfileResult
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public UserStatus Status { get; init; }
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }

    public static GetMyProfileResult FromEntity(User entity)
    {
        return new GetMyProfileResult
        {
            Id = entity.Id,
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NickName = entity.NickName,
            Sex = entity.Sex,
            Status = entity.Status,
            BirthDate = entity.BirthDate,
            AvatarUrl = entity.AvatarUrl
        };
    }
}
