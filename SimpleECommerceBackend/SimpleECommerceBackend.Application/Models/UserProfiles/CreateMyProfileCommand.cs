using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Application.Enums;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public class CreateMyProfileCommand
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public DateOnly BirthDate { get; init; }
}