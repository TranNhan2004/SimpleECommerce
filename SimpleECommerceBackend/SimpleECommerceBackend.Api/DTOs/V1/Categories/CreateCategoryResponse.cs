using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.Models.Categories;

public class CreateCategoryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;
    public Guid AdminId { get; init; }
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
            AdminId = result.AdminId,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };
    }
}