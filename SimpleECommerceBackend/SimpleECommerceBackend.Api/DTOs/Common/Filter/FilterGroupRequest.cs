using SimpleECommerceBackend.Domain.Enums.Common;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public class FilterGroupRequest
{
    public string Logic { get; init; } = null!;
    public List<FilterGroupNodeRequest> Children { get; init; } = [];

    public static FilterGroup ToModel(FilterGroupRequest request)
    {
        return new FilterGroup
        {
            Logic = EnumUtils.FromDisplayValue<FilterGroupLogic>(request.Logic),
            Children = [.. request.Children.Select(FilterGroupNodeRequest.ToModel)]
        };
    }
}

