namespace SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;

public class UpdateMyProfileRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? NickName { get; init; }
    public string? Sex { get; init; }
    public DateOnly? BirthDate { get; init; }
}