# AI Event Processing System

Backend system for ingesting and querying AI/IoT telemetry events using ASP.NET Core, EF Core, and PostgreSQL.

---

## Tech Stack

- ASP.NET Core 10 Web API
- Entity Framework Core
- PostgreSQL
- Swagger
- xUnit
- FluentAssertions

---

## Features

- Single event ingestion
- Bulk event ingestion (up to 10,000 records)
- Event filtering
- Event summary analytics
- JSON metadata storage
- Optimized EF Core queries
- Integration testing

---

## API Endpoints

### POST /api/events
Insert single event

### POST /api/events/bulk
Insert bulk events

### GET /api/events
Filter events by:
- deviceId
- status
- date range

### GET /api/events/summary
Returns:
- average temperature
- min temperature
- max temperature
- count per device

---

## Performance Optimizations

- AsNoTracking used for read-only queries
- Batch insertion using AddRangeAsync
- Reduced database round trips
- Indexed timestamp and device fields
- JSONB column for metadata
- Avoided N+1 queries

---

## Trade-offs

- Used EF Core batching instead of raw SQL COPY for simplicity and maintainability
- Metadata stored as JSONB for flexibility over strict schema normalization

---

## Assumptions

- Device IDs are unique
- Events are immutable after insertion
- Metadata schema may evolve over time

---

## Run Project

### Start PostgreSQL

```bash
docker compose up -d
cd API
dotnet watch run
dotnet test