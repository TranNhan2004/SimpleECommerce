namespace SimpleECommerceBackend.Api.DTOs.V1_0.Category;

public class CreateCategoryForAdminRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}