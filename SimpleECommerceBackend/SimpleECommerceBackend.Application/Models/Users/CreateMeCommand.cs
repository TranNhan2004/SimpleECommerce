using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Users;

public class CreateMeCommand
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public DateOnly BirthDate { get; init; }
}
