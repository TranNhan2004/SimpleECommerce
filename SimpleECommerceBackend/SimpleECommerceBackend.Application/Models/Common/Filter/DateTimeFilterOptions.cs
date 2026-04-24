using SimpleECommerceBackend.Application.Enums;

namespace SimpleECommerceBackend.Application.Models.Common.Filter;

public class DateTimeFilterOptions
{
    public int OffsetMinutes { get; init; }
    public TemporalPartType TemporalPartType { get; init; }
}