namespace SimpleECommerceBackend.Api.DTOs.V1_0.UserProfiles;

public class GetMyProfileResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public string Status { get; init; } = null!;
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }
}