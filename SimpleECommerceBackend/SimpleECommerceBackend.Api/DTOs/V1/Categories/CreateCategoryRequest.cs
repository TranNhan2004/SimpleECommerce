namespace SimpleECommerceBackend.Application.Models.Categories;

public class CreateCategoryRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }

    public static CreateCategoryCommand ToCommand(CreateCategoryRequest request)
    {
        return new CreateCategoryCommand
        {
            Name = request.Name,
            Description = request.Description
        };
    }
}