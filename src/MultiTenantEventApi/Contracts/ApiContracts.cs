namespace MultiTenantEventApi.Contracts;

public sealed record EventResponse(Guid Id, string Title, DateTime StartsAtUtc, int Capacity);
public sealed record BookingResponse(Guid Id, string Name, string Email, DateTime CreatedAtUtc);
public sealed record CreateBookingRequest(string Name, string Email);
