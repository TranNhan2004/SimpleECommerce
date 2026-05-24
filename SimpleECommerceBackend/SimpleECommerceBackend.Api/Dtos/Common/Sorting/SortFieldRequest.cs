namespace SimpleECommerceBackend.Api.Dtos.Common.Sorting;

public class SortFieldRequest
{
    public string FieldName { get; init; } = null!;
    public bool IsAscending { get; init; }
}