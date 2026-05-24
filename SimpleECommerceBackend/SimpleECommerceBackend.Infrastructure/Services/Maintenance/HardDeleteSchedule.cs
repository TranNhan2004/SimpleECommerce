namespace SimpleECommerceBackend.Infrastructure.Services.Maintenance;

public static class HardDeleteSchedule
{
    private static readonly TimeOnly[] RunTimes =
    [
        new(0, 0, 0),
        new(12, 0, 0)
    ];

    public static DateTimeOffset GetNextRun(DateTimeOffset now, TimeZoneInfo timeZone)
    {
        var localNow = TimeZoneInfo.ConvertTime(now, timeZone);
        var localDate = DateOnly.FromDateTime(localNow.DateTime);

        foreach (var runTime in RunTimes)
        {
            var candidate = CreateOccurrence(localDate, runTime, timeZone);
            if (candidate >= localNow)
            {
                return candidate;
            }
        }

        return CreateOccurrence(localDate.AddDays(1), RunTimes[0], timeZone);
    }

    private static DateTimeOffset CreateOccurrence(
        DateOnly date,
        TimeOnly time,
        TimeZoneInfo timeZone
    )
    {
        var localDateTime = date.ToDateTime(time, DateTimeKind.Unspecified);
        var offset = timeZone.GetUtcOffset(localDateTime);

        return new DateTimeOffset(localDateTime, offset);
    }
}
