namespace MultiTenantEventApi.Models;

public sealed class CommunityEvent
{
    public Guid Id { get; set; }
    public Guid OrganisationId { get; set; }
    public required string Title { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public int Capacity { get; set; }
    public Organisation? Organisation { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public bool HasCapacityFor(int currentBookingCount) =>
        Capacity == 0 || currentBookingCount < Capacity;
}
