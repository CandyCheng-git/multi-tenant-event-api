using Microsoft.EntityFrameworkCore;
using MultiTenantEventApi.Models;
using MultiTenantEventApi.Time;

namespace MultiTenantEventApi.Data;

public static class DevelopmentSeeder
{
    public static readonly Guid NorthsideOrganisationId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid SouthsideOrganisationId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid TechNightEventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid CommunityLunchEventId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Organisations.AnyAsync(x => x.Id == NorthsideOrganisationId))
            return;

        db.Organisations.AddRange(
            new Organisation { Id = NorthsideOrganisationId, Name = "Northside Community Collective" },
            new Organisation { Id = SouthsideOrganisationId, Name = "Southside Neighbourhood Network" });

        db.Events.AddRange(
            new CommunityEvent
            {
                Id = TechNightEventId,
                OrganisationId = NorthsideOrganisationId,
                Title = "Neighbourhood Tech Night",
                StartsAtUtc = MelbourneTimeConverter.LocalToUtc(new DateTime(2026, 10, 15, 18, 0, 0)),
                Capacity = 12
            },
            new CommunityEvent
            {
                Id = CommunityLunchEventId,
                OrganisationId = SouthsideOrganisationId,
                Title = "Community Lunch",
                StartsAtUtc = MelbourneTimeConverter.LocalToUtc(new DateTime(2026, 10, 18, 12, 30, 0)),
                Capacity = 0
            });

        await db.SaveChangesAsync();
    }
}
