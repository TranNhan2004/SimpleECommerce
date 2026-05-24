using FluentAssertions;
using SimpleECommerceBackend.Infrastructure.Services.Maintenance;

namespace SimpleECommerceBackend.Infrastructure.Tests.Services.Maintenance;

public class HardDeleteScheduleTests
{
    private static readonly TimeZoneInfo TestTimeZone = TimeZoneInfo.CreateCustomTimeZone(
        id: "UtcPlus7",
        baseUtcOffset: TimeSpan.FromHours(7),
        displayName: "UTC+07",
        standardDisplayName: "UTC+07"
    );

    [Fact]
    public void GetNextRun_ShouldReturnSameTimestamp_WhenNowIsExactScheduledTime()
    {
        var now = new DateTimeOffset(2026, 5, 8, 12, 0, 0, TimeSpan.FromHours(7));

        var nextRun = HardDeleteSchedule.GetNextRun(now, TestTimeZone);

        nextRun.Should().Be(now);
    }

    [Fact]
    public void GetNextRun_ShouldReturnNoon_WhenNowIsBeforeNoon()
    {
        var now = new DateTimeOffset(2026, 5, 8, 10, 15, 0, TimeSpan.FromHours(7));

        var nextRun = HardDeleteSchedule.GetNextRun(now, TestTimeZone);

        nextRun.Should().Be(new DateTimeOffset(2026, 5, 8, 12, 0, 0, TimeSpan.FromHours(7)));
    }

    [Fact]
    public void GetNextRun_ShouldReturnNextMidnight_WhenNowIsAfterNoon()
    {
        var now = new DateTimeOffset(2026, 5, 8, 12, 0, 1, TimeSpan.FromHours(7));

        var nextRun = HardDeleteSchedule.GetNextRun(now, TestTimeZone);

        nextRun.Should().Be(new DateTimeOffset(2026, 5, 9, 0, 0, 0, TimeSpan.FromHours(7)));
    }
}
