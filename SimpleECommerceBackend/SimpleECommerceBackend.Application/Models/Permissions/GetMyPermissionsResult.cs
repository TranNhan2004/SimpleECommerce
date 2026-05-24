namespace SimpleECommerceBackend.Application.Models.Permissions;

public class GetMyPermissionsResult
{
    public IReadOnlyList<string> Permissions { get; init; } = [];
}
