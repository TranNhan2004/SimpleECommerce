using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public class CreateMyProfileResult
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public DateOnly BirthDate { get; init; }

    public static CreateMyProfileResult FromEntity(User entity)
    {
        return new CreateMyProfileResult
        {
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NickName = entity.NickName,
            Sex = entity.Sex,
            BirthDate = entity.BirthDate
        };
    }
}
