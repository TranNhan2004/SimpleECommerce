namespace SimpleECommerceBackend.Api.DTOs.V1_0.Category;

public class CreateCategoryForAdminResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;
    public Guid AdminId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}