using SimpleECommerceBackend.Domain.Enums.Common;

namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public class DateTimeFilterOptions
{
    public int OffsetMinutes { get; init; }
    public TemporalPartType TemporalPartType { get; init; }
}