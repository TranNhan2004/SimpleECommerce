namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public class UpdateMyProfileCommand
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? NickName { get; init; }
    public string? Sex { get; init; }
    public DateOnly? BirthDate { get; init; }
}