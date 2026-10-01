# Multi-Tenant Event API

[![CI](https://github.com/CandyCheng-git/multi-tenant-event-api/actions/workflows/ci.yml/badge.svg)](https://github.com/CandyCheng-git/multi-tenant-event-api/actions/workflows/ci.yml)

A public backend portfolio project built with **C# / .NET 9**, **Entity Framework Core**, **PostgreSQL**, **Docker Compose** and **xUnit**.

It demonstrates a small multi-tenant event-booking API with server-side organisation isolation, timezone-aware event data, capacity enforcement and regression tests.

> This is an independent portfolio implementation and does not reproduce a private or active technical-assessment repository.

## Highlights

- REST API design with explicit HTTP behaviour
- EF Core global query filters for organisation isolation
- PostgreSQL persistence through Npgsql
- Dockerised local development
- `404 Not Found` for foreign-organisation resources
- `409 Conflict` for full events and duplicate bookings
- `Capacity = 0` as unlimited
- Melbourne daylight-saving-aware UTC conversion
- xUnit regression tests
- GitHub Actions CI

## Domain

```text
Organisation
  └── CommunityEvent
       └── Booking
```

## Request flow

```text
HTTP request
  + X-Organisation-Id
        |
        v
ASP.NET Core Minimal API
        |
        v
IOrganisationContext
        |
        v
AppDbContext
  + EF Core global query filters
        |
        v
PostgreSQL
```

## API

```http
GET  /health
GET  /api/events
GET  /api/events/{id}
GET  /api/events/{id}/bookings
POST /api/events/{id}/bookings
```

All `/api` routes require:

```http
X-Organisation-Id: <guid>
```

| Scenario | Result |
|---|---|
| Missing/invalid organisation header | `400 Bad Request` |
| Event belongs to current organisation | `200 OK` |
| Event missing or belongs to another organisation | `404 Not Found` |
| Booking created | `201 Created` |
| Duplicate email | `409 Conflict` |
| Positive-capacity event is full | `409 Conflict` |
| Capacity is zero | Unlimited |

## Run

```bash
docker compose up --build
```

API: `http://localhost:8080`

OpenAPI document in Development: `http://localhost:8080/openapi/v1.json`

## Demo data

Northside organisation:
`11111111-1111-1111-1111-111111111111`

Northside event:
`aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`

Southside organisation:
`22222222-2222-2222-2222-222222222222`

Southside event:
`bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb`

Example:

```bash
curl -H "X-Organisation-Id: 11111111-1111-1111-1111-111111111111" \
  http://localhost:8080/api/events
```

Create a booking:

```bash
curl -X POST \
  -H "Content-Type: application/json" \
  -H "X-Organisation-Id: 11111111-1111-1111-1111-111111111111" \
  -d '{"name":"Alex Wong","email":"alex@example.com"}' \
  http://localhost:8080/api/events/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/bookings
```

A ready-to-run request collection is included in `requests.http`.

## Test

```bash
dotnet test
```

The suite covers capacity boundaries, unlimited events, Melbourne standard/daylight-saving conversion, and cross-organisation isolation for both events and bookings.

## Structure

```text
.
├── .github/workflows/ci.yml
├── docs/architecture.md
├── src/MultiTenantEventApi/
├── tests/MultiTenantEventApi.Tests/
├── docker-compose.yml
├── requests.http
└── MultiTenantEventApi.sln
```

## Known limitation

Capacity checking is `COUNT -> CHECK -> INSERT`, so the final seat is not concurrency-safe under simultaneous requests. A database-backed transactional strategy is the next production-hardening step.
