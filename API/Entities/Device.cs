namespace API.Entities;

public class Device
{
    public Guid Id { get; set; }

    public string DeviceIdText { get; set; } = default!;

    public ICollection<Event> Events { get; set; }
        = new List<Event>();
}