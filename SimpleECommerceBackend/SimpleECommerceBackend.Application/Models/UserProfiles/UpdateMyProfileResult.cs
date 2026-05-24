using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public class UpdateMyProfileResult
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public UserStatus Status { get; init; }
    public DateOnly BirthDate { get; init; }

    public static UpdateMyProfileResult FromEntity(User entity)
    {
        return new UpdateMyProfileResult
        {
            Id = entity.Id,
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NickName = entity.NickName,
            Sex = entity.Sex,
            Status = entity.Status,
            BirthDate = entity.BirthDate
        };
    }
}
