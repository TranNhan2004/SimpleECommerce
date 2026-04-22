using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Enums.Common;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public class CreateMyProfileCommand
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public DateOnly BirthDate { get; init; }
}