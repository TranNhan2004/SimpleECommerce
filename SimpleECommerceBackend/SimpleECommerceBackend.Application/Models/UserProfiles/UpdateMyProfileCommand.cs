namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public class UpdateMyProfileCommand
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public string Sex { get; init; } = null!;
    public DateOnly BirthDate { get; init; }
}