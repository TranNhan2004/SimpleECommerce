namespace SimpleECommerceBackend.Application.Models.Categories;

public class CreateCategoryCommand
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}