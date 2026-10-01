using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MultiTenantEventApi.Data;
using MultiTenantEventApi.Models;
using MultiTenantEventApi.Tenancy;
using Xunit;

namespace MultiTenantEventApi.Tests;

public sealed class TenantIsolationTests
{
    [Fact]
    public async Task Event_query_hides_foreign_organisation_rows()
    {
        var root = new InMemoryDatabaseRoot();
        var dbName = Guid.NewGuid().ToString();
        var orgA = Guid.NewGuid();
        var orgB = Guid.NewGuid();
        var ownId = Guid.NewGuid();
        var foreignId = Guid.NewGuid();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName, root)
            .Options;

        await using (var seed = new AppDbContext(options, new StubOrganisationContext(orgA)))
        {
            seed.Events.AddRange(
                new CommunityEvent { Id = ownId, OrganisationId = orgA, Title = "A", StartsAtUtc = DateTime.UtcNow, Capacity = 10 },
                new CommunityEvent { Id = foreignId, OrganisationId = orgB, Title = "B", StartsAtUtc = DateTime.UtcNow, Capacity = 10 });
            await seed.SaveChangesAsync();
        }

        await using var scoped = new AppDbContext(options, new StubOrganisationContext(orgA));
        var visibleIds = await scoped.Events.Select(x => x.Id).ToListAsync();

        Assert.Contains(ownId, visibleIds);
        Assert.DoesNotContain(foreignId, visibleIds);
        Assert.Null(await scoped.Events.SingleOrDefaultAsync(x => x.Id == foreignId));
    }

    [Fact]
    public async Task Booking_query_hides_foreign_organisation_rows()
    {
        var root = new InMemoryDatabaseRoot();
        var dbName = Guid.NewGuid().ToString();
        var orgA = Guid.NewGuid();
        var orgB = Guid.NewGuid();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName, root)
            .Options;

        await using (var seed = new AppDbContext(options, new StubOrganisationContext(orgA)))
        {
            seed.Bookings.AddRange(
                new Booking { Id = Guid.NewGuid(), OrganisationId = orgA, EventId = Guid.NewGuid(), Name = "Alex", Email = "alex@example.com", NormalizedEmail = "ALEX@EXAMPLE.COM", CreatedAtUtc = DateTime.UtcNow },
                new Booking { Id = Guid.NewGuid(), OrganisationId = orgB, EventId = Guid.NewGuid(), Name = "Jordan", Email = "jordan@example.com", NormalizedEmail = "JORDAN@EXAMPLE.COM", CreatedAtUtc = DateTime.UtcNow });
            await seed.SaveChangesAsync();
        }

        await using var scoped = new AppDbContext(options, new StubOrganisationContext(orgA));
        var bookings = await scoped.Bookings.ToListAsync();

        Assert.Single(bookings);
        Assert.Equal(orgA, bookings[0].OrganisationId);
    }

    private sealed class StubOrganisationContext(Guid id) : IOrganisationContext
    {
        public Guid OrganisationId { get; } = id;
    }
}
