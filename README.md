# AI Event Processing System

Backend system for ingesting and querying AI/IoT telemetry events using ASP.NET Core, EF Core, RabbitMq and SQL Server

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
