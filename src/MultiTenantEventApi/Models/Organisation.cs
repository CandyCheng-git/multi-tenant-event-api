namespace MultiTenantEventApi.Models;

public sealed class Organisation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<CommunityEvent> Events { get; set; } = new List<CommunityEvent>();
}
