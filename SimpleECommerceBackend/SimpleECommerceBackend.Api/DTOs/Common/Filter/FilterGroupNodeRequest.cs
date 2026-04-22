namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public class FilterGroupNodeRequest
{
    public int? CriterionIndex { get; init; }
    public FilterGroupRequest? Group { get; init; }

    public static FilterGroupNode ToModel(FilterGroupNodeRequest request)
    {
        return new FilterGroupNode
        {
            CriterionIndex = request.CriterionIndex,
            Group = request.Group is null ? null : FilterGroupRequest.ToModel(request.Group)
        };
    }
}
