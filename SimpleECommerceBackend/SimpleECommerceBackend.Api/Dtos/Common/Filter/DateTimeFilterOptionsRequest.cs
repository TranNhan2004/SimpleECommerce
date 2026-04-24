using SimpleECommerceBackend.Application.Enums;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.Common.Filter;

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