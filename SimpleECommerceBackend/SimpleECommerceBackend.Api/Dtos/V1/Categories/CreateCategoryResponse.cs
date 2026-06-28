using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Categories;

public class CreateCategoryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public static CreateCategoryResponse FromResult(CreateCategoryResult result)
    {
        return new CreateCategoryResponse
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.Description,
            Status = EnumUtils.ToDisplayValue(result.Status),
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };
    }
}