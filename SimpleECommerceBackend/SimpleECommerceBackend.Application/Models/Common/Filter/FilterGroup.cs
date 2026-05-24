using SimpleECommerceBackend.Application.Enums;

namespace SimpleECommerceBackend.Application.Models.Common.Filter;

public class FilterGroup
{
    public FilterGroupLogic Logic { get; init; }
    public List<FilterGroupNode> Children { get; init; } = [];
}

