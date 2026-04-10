namespace SimpleECommerceBackend.Api.DTOs.UserProfiles;

public class UpdateMyProfileRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; } = null!;
    public string Sex { get; init; } = null!;
    public DateOnly BirthDate { get; init; }
}