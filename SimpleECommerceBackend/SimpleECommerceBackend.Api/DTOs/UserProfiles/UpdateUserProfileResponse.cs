namespace SimpleECommerceBackend.Api.DTOs.UserProfiles;

public class UpdateUserProfileResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; }
    public string Status { get; init; }
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }
}