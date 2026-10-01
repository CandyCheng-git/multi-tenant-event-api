using MultiTenantEventApi.Models;
using Xunit;

namespace MultiTenantEventApi.Tests;

public sealed class CommunityEventTests
{
    [Fact]
    public void Zero_capacity_is_unlimited()
    {
        var e = CreateEvent(0);
        Assert.True(e.HasCapacityFor(10_000));
    }

    [Fact]
    public void Positive_capacity_rejects_when_full()
    {
        var e = CreateEvent(2);
        Assert.False(e.HasCapacityFor(2));
    }

    [Fact]
    public void Positive_capacity_accepts_when_below_limit()
    {
        var e = CreateEvent(2);
        Assert.True(e.HasCapacityFor(1));
    }

    private static CommunityEvent CreateEvent(int capacity) => new()
    {
        Id = Guid.NewGuid(),
        OrganisationId = Guid.NewGuid(),
        Title = "Test event",
        StartsAtUtc = DateTime.UtcNow,
        Capacity = capacity
    };
}
