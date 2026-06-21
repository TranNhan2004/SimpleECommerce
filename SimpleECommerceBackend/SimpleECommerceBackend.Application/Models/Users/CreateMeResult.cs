using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Users;

public class CreateMeResult
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public DateOnly BirthDate { get; init; }

    public static CreateMeResult FromEntity(User entity)
    {
        return new CreateMeResult
        {
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            NickName = entity.NickName,
            Sex = entity.Sex,
            BirthDate = entity.BirthDate
        };
    }
}
