using Microsoft.EntityFrameworkCore;
using MultiTenantEventApi.Contracts;
using MultiTenantEventApi.Data;
using MultiTenantEventApi.Models;
using MultiTenantEventApi.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IOrganisationContext, HeaderOrganisationContext>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")
        ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DevelopmentSeeder.SeedAsync(db);
}

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        var valid = context.Request.Headers.TryGetValue("X-Organisation-Id", out var raw)
            && Guid.TryParse(raw.ToString(), out _);

        if (!valid)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(
                new { error = "A valid X-Organisation-Id header is required." });
            return;
        }
    }

    await next();
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

var events = app.MapGroup("/api/events");

events.MapGet("/", async (AppDbContext db) =>
{
    var items = await db.Events.AsNoTracking()
        .OrderBy(x => x.StartsAtUtc)
        .Select(x => new EventResponse(x.Id, x.Title, x.StartsAtUtc, x.Capacity))
        .ToListAsync();

    return Results.Ok(items);
});

events.MapGet("/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var item = await db.Events.AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new EventResponse(x.Id, x.Title, x.StartsAtUtc, x.Capacity))
        .SingleOrDefaultAsync();

    return item is null ? Results.NotFound() : Results.Ok(item);
});

events.MapGet("/{id:guid}/bookings", async (Guid id, AppDbContext db) =>
{
    if (!await db.Events.AnyAsync(x => x.Id == id))
        return Results.NotFound();

    var bookings = await db.Bookings.AsNoTracking()
        .Where(x => x.EventId == id)
        .OrderBy(x => x.CreatedAtUtc)
        .Select(x => new BookingResponse(x.Id, x.Name, x.Email, x.CreatedAtUtc))
        .ToListAsync();

    return Results.Ok(bookings);
});

events.MapPost("/{id:guid}/bookings", async (
    Guid id,
    CreateBookingRequest request,
    AppDbContext db,
    IOrganisationContext organisationContext) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        return Results.BadRequest(new { error = "Name and email are required." });

    var communityEvent = await db.Events.SingleOrDefaultAsync(x => x.Id == id);
    if (communityEvent is null)
        return Results.NotFound();

    var normalizedEmail = request.Email.Trim().ToUpperInvariant();

    if (await db.Bookings.AnyAsync(x => x.EventId == id && x.NormalizedEmail == normalizedEmail))
        return Results.Conflict(new { error = "This email is already booked for the event." });

    var count = await db.Bookings.CountAsync(x => x.EventId == id);
    if (!communityEvent.HasCapacityFor(count))
        return Results.Conflict(new { error = "This event is full." });

    var booking = new Booking
    {
        Id = Guid.NewGuid(),
        OrganisationId = organisationContext.OrganisationId,
        EventId = id,
        Name = request.Name.Trim(),
        Email = request.Email.Trim(),
        NormalizedEmail = normalizedEmail,
        CreatedAtUtc = DateTime.UtcNow
    };

    db.Bookings.Add(booking);
    await db.SaveChangesAsync();

    var response = new BookingResponse(booking.Id, booking.Name, booking.Email, booking.CreatedAtUtc);
    return Results.Created($"/api/events/{id}/bookings/{booking.Id}", response);
});

app.Run();

public partial class Program;
