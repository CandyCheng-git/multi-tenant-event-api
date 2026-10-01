using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EventDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/events", async (EventDbContext db) =>
    Results.Ok(await db.Events.AsNoTracking().OrderBy(x => x.StartsAtUtc).ToListAsync()));

app.Run();

public sealed class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
{
    public DbSet<CommunityEvent> Events => Set<CommunityEvent>();
}

public sealed class CommunityEvent
{
    public Guid Id { get; set; }
    public Guid OrganisationId { get; set; }
    public required string Title { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public int Capacity { get; set; }
}
