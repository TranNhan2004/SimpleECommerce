namespace SimpleECommerceBackend.Api.DTOs.V1.Categories;

public class GetAllCategoriesResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;
}