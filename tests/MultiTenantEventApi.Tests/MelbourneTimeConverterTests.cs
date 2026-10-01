using MultiTenantEventApi.Time;
using Xunit;

namespace MultiTenantEventApi.Tests;

public sealed class MelbourneTimeConverterTests
{
    [Fact]
    public void September_local_time_uses_standard_time()
    {
        var local = new DateTime(2026, 9, 24, 18, 0, 0);
        var utc = MelbourneTimeConverter.LocalToUtc(local);
        Assert.Equal(new DateTime(2026, 9, 24, 8, 0, 0, DateTimeKind.Utc), utc);
    }

    [Fact]
    public void October_local_time_uses_daylight_saving_time()
    {
        var local = new DateTime(2026, 10, 17, 18, 0, 0);
        var utc = MelbourneTimeConverter.LocalToUtc(local);
        Assert.Equal(new DateTime(2026, 10, 17, 7, 0, 0, DateTimeKind.Utc), utc);
    }
}
