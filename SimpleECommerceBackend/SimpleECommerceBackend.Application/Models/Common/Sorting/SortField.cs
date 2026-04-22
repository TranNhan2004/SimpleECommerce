namespace SimpleECommerceBackend.Api.DTOs.Common.Sorting;

public class SortField
{
    public string FieldName { get; init; } = null!;
    public bool IsAscending { get; init; }
}