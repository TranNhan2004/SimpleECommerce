using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.Models.Categories;

public class UpdateCategoryRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;

    public static UpdateCategoryCommand ToCommand(UpdateCategoryRequest request)
    {
        return new UpdateCategoryCommand
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            Status = EnumUtils.FromDisplayValue<CategoryStatus>(request.Status)
        };
    }
}