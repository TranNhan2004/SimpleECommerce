using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Categories;

public class GetCategoryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;

    public static GetCategoryResponse FromResult(GetCategoryResult result)
    {
        return new GetCategoryResponse
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.Description,
            Status = EnumUtils.ToDisplayValue(result.Status)
        };
    }
}