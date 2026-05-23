namespace API.Entities;

public class Event
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public Device Device { get; set; } = default!;

    public DateTime Timestamp { get; set; }

    public decimal Temperature { get; set; }

    public string Status { get; set; } = default!;

    public string Metadata { get; set; } = default!;
}