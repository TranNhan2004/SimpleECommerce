namespace SimpleECommerceBackend.Application.Models.Categories;

public class UpdateCategoryCommand
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;
}