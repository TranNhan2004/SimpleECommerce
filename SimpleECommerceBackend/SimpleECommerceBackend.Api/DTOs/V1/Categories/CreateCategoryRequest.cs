namespace SimpleECommerceBackend.Api.DTOs.V1.Categories;

public class CreateCategoryRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}