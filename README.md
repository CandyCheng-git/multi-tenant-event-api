# Multi-Tenant Event API

Independent portfolio project built with **C# / .NET**, **Entity Framework Core**, **PostgreSQL**, **Docker**, and **xUnit**.

The goal is to demonstrate production-style backend engineering around multi-tenant data isolation, REST API design, validation, timezone handling, and regression testing.

> This repository is an original portfolio project. It is not a copy of any employer, volunteer organisation, or technical-assessment repository.

## Planned capabilities

- Organisation-scoped events and bookings
- Tenant isolation using EF Core global query filters
- REST endpoints with explicit HTTP status behaviour
- Capacity enforcement for limited and unlimited events
- Australia/Melbourne timezone conversion with daylight-saving support
- PostgreSQL persistence
- Docker Compose development environment
- xUnit regression and API tests

## Proposed domain model

```text
Organisation
  └── Event
       └── Booking
```

Each request will operate within an organisation context. Data belonging to one organisation must never be visible to another organisation.

## Planned API

```http
GET  /api/events
GET  /api/events/{id}
GET  /api/events/{id}/bookings
POST /api/events/{id}/bookings
```

Expected behaviour will include:

- `404 Not Found` for resources outside the current organisation scope
- `409 Conflict` when a capacity-limited event is full
- `201 Created` for successful bookings
- `Capacity = 0` treated as unlimited

## Engineering focus

The project will favour:

1. small, explainable changes;
2. server-side tenant enforcement;
3. regression tests for security and business rules;
4. minimal DTOs rather than exposing persistence entities directly;
5. documented limitations and design decisions.

## Status

Initial project setup in progress.
