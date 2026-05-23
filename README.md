# AI Event Processing System

Backend system for ingesting and querying AI/IoT telemetry events using ASP.NET Core, EF Core, RabbitMQ and SQL Server

---
## Architecture Decisions

- Chose **event-driven architecture** using RabbitMQ to decouple API from database operations
- API layer is responsible only for validation and message publishing
- A separate consumer handles persistence to SQL Server
- EF Core used for ORM-based data access for maintainability and rapid development
- JSON metadata stored as serialized string to support flexible schema evolution
- Docker used for running infrastructure services (SQL Server, RabbitMQ)

---

## Tech Stack

- ASP.NET Core 10 Web API
- Entity Framework Core
- SQL Server
- RabbitMq
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
## Trade-offs

- Introduced **eventual consistency** by using RabbitMQ instead of direct DB writes
- Chose EF Core over raw SQL bulk operations for readability and maintainability
- Metadata stored as JSON string instead of normalized relational tables for flexibility
- Additional infrastructure complexity due to RabbitMQ setup
- Slight increase in latency due to asynchronous processing

---

## Assumptions

- Device IDs are globally unique identifiers
- Events are immutable once stored in the system
- RabbitMQ consumer service is always running for full data pipeline functionality
- Message delivery is reliable (acknowledged after processing)
- JSON metadata structure may evolve without schema migration
- System runs in a trusted environment (no external authentication implemented)

----
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
### POST /api/events/publish
-Publish event to RabbitMQ queue (asynchronous processing)

## Performance Optimizations

- AsNoTracking used for read-only queries
- Batch insertion using AddRangeAsync
- Reduced database round trips
- Indexed timestamp and device fields
- Avoided N+1 queries

---

## Run Project

### Start SQL Server (Docker)

docker run -e "ACCEPT_EULA=Y" \
-e "MSSQL_SA_PASSWORD=YourPassword123!" \
-p 1433:1433 \
--name sqlserver \
-d mcr.microsoft.com/mssql/server:2022-latest

###Start RabbitMQ

docker run -d \
--hostname rabbit \
-p 5672:5672 -p 15672:15672 \
rabbitmq:3-management

###Run Application

cd API
dotnet restore
dotnet run
dotnet test
