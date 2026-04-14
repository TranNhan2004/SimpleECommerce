namespace SimpleECommerceBackend.Api.DTOs.V1_0.UserProfiles;

public class CreateMyProfileRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public DateOnly BirthDate { get; init; }
}