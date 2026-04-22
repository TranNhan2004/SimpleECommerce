using SimpleECommerceBackend.Domain.Enums.Common;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public class DateTimeFilterOptionsRequest
{
    public int OffsetMinutes { get; init; }
    public string TemporalPartType { get; init; } = null!;

    public static DateTimeFilterOptions ToModel(DateTimeFilterOptionsRequest request)
    {
        return new DateTimeFilterOptions
        {
            OffsetMinutes = request.OffsetMinutes,
            TemporalPartType = EnumUtils.FromDisplayValue<TemporalPartType>(request.TemporalPartType)
        };
    }
}