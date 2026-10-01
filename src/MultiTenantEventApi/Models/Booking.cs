namespace MultiTenantEventApi.Models;

public sealed class Booking
{
    public Guid Id { get; set; }
    public Guid OrganisationId { get; set; }
    public Guid EventId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string NormalizedEmail { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public CommunityEvent? Event { get; set; }
}
