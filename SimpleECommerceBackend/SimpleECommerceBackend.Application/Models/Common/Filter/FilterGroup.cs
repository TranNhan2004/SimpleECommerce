using SimpleECommerceBackend.Domain.Enums.Common;

namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public class FilterGroup
{
    public FilterGroupLogic Logic { get; init; }
    public List<FilterGroupNode> Children { get; init; } = [];
}

