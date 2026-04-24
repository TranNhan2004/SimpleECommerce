using SimpleECommerceBackend.Application.Enums;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.Common.Filter;

public class FilterCriterionRequest
{
    public string FieldName { get; set; } = null!;
    public string Operator { get; set; } = null!;
    public string FieldType { get; set; } = null!;
    public List<string> Values { get; set; } = null!;
    public DateTimeFilterOptionsRequest? DateTimeFilterOptions { get; set; }

    public static FilterCriterion ToModel(FilterCriterionRequest request)
    {
        return new FilterCriterion
        {
            FieldName = request.FieldName,
            Operator = EnumUtils.FromDisplayValue<FilterOperator>(request.Operator),
            FieldType = EnumUtils.FromDisplayValue<FieldType>(request.FieldType),
            Values = request.Values,
            DateTimeFilterOptions = request.DateTimeFilterOptions is null
                ? null
                : DateTimeFilterOptionsRequest.ToModel(request.DateTimeFilterOptions)
        };
    }
}