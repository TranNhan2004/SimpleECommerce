using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Users;

public class GetMeResult
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
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
