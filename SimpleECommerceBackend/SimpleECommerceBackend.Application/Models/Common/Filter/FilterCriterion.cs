using SimpleECommerceBackend.Application.Enums;

namespace SimpleECommerceBackend.Application.Models.Common.Filter;

public class FilterCriterion
{
    public string FieldName { get; set; } = null!;
    public FilterOperator Operator { get; set; }
    public FieldType FieldType { get; set; }
    public List<string> Values { get; set; } = null!;
    public DateTimeFilterOptions? DateTimeFilterOptions { get; set; }
}