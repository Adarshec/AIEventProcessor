using API.Data;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using API.Entities;

namespace API.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateEventRequest request)
    {
        var device = await _context.Devices
            .FirstOrDefaultAsync(x =>
                x.DeviceIdText == request.DeviceId);

if (device == null)
{
    device = new Device
    {
        Id = Guid.NewGuid(),
        DeviceIdText = request.DeviceId
    };

    _context.Devices.Add(device);

    await _context.SaveChangesAsync();
}

        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            DeviceId = device.Id,
            Timestamp = request.Timestamp,
            Temperature = request.Temperature,
            Status = request.Status,
            Metadata = JsonSerializer.Serialize(request.Metadata)
        };

        _context.Events.Add(newEvent);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Event stored successfully"
        });
    }
[HttpGet]
public async Task<IActionResult> Get(
    [FromQuery] string? deviceId,
    [FromQuery] string? status,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to)
{
    var query = _context.Events
        .Include(x => x.Device)
        .AsNoTracking()
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(deviceId))
    {
        query = query.Where(x =>
            x.Device.DeviceIdText == deviceId);
    }

    if (!string.IsNullOrWhiteSpace(status))
    {
        query = query.Where(x =>
            x.Status == status);
    }

    if (from.HasValue)
{
    var fromUtc = DateTime.SpecifyKind(
        from.Value,
        DateTimeKind.Utc);

    query = query.Where(x =>
        x.Timestamp >= fromUtc);
}

if (to.HasValue)
{
    var toUtc = DateTime.SpecifyKind(
        to.Value,
        DateTimeKind.Utc);

    query = query.Where(x =>
        x.Timestamp <= toUtc);
}
    var result = await query
        .OrderByDescending(x => x.Timestamp)
        .Select(x => new
        {
            x.Id,
            DeviceId = x.Device.DeviceIdText,
            x.Timestamp,
            x.Temperature,
            x.Status,
            Metadata = x.Metadata
        })
        .ToListAsync();

    return Ok(result);
}
[HttpGet("summary")]
public async Task<IActionResult> GetSummary()
{
    var result = await _context.Events
        .AsNoTracking()
        .Include(x => x.Device)
        .GroupBy(x => x.Device.DeviceIdText)
        .Select(g => new
        {
            DeviceId = g.Key,
            Count = g.Count(),
            AverageTemperature = g.Average(x => x.Temperature),
            MaxTemperature = g.Max(x => x.Temperature),
            MinTemperature = g.Min(x => x.Temperature)
        })
        .ToListAsync();

    return Ok(result);
}
[HttpPost("bulk")]
public async Task<IActionResult> BulkInsert(
    List<CreateEventRequest> requests)
{
    if (requests.Count > 10000)
    {
        return BadRequest(
            "Maximum 10,000 events allowed");
    }

    var deviceIds = requests
        .Select(x => x.DeviceId)
        .Distinct()
        .ToList();

    var existingDevices = await _context.Devices
        .Where(x => deviceIds.Contains(x.DeviceIdText))
        .ToListAsync();

    var existingDeviceMap = existingDevices
        .ToDictionary(x => x.DeviceIdText);

    var newDevices = new List<Device>();

    foreach (var deviceId in deviceIds)
    {
        if (!existingDeviceMap.ContainsKey(deviceId))
        {
            var device = new Device
            {
                Id = Guid.NewGuid(),
                DeviceIdText = deviceId
            };

            newDevices.Add(device);

            existingDeviceMap[deviceId] = device;
        }
    }

    if (newDevices.Any())
    {
        await _context.Devices.AddRangeAsync(newDevices);

        await _context.SaveChangesAsync();
    }

    var events = requests.Select(request => new Event
    {
        Id = Guid.NewGuid(),
        DeviceId = existingDeviceMap[
            request.DeviceId].Id,

        Timestamp = request.Timestamp,

        Temperature = request.Temperature,

        Status = request.Status,

        Metadata = JsonSerializer.Serialize(
            request.Metadata)
    }).ToList();

    await _context.Events.AddRangeAsync(events);

    await _context.SaveChangesAsync();

    return Ok(new
    {
        Inserted = events.Count
    });
}
}