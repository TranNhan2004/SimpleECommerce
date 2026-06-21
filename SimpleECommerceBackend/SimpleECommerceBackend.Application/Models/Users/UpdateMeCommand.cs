using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Users;

public class UpdateMeCommand
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? NickName { get; init; }
    public Sex? Sex { get; init; }
    public DateOnly? BirthDate { get; init; }
}
