# Architecture decisions

## Server-side organisation isolation

Every `/api` request requires `X-Organisation-Id`. The header is exposed through `IOrganisationContext`, and `AppDbContext` applies EF Core global query filters to events and bookings.

Knowing a GUID identifies a resource; it does not authorise access to it.

## Foreign resources return 404

A row that belongs to another organisation is filtered out before the endpoint can see it, so the external behaviour matches a missing resource.

## UTC storage and Melbourne conversion

Local Melbourne wall-clock values are converted using the `Australia/Melbourne` timezone rules. The tests cover standard time and daylight-saving time.

## Capacity

Positive capacity allows bookings only while `currentBookingCount < capacity`. Capacity `0` means unlimited.

## Duplicate booking protection

The API checks duplicates and the database model has a unique index over `OrganisationId + EventId + NormalizedEmail`.

## Known concurrency boundary

Capacity uses `COUNT -> CHECK -> INSERT`. Those operations are not atomic, so simultaneous requests could race for the final seat. A database-backed transactional strategy would be the next hardening step.
