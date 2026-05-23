using API.DTOs;

namespace API.DTOs;

public class CreateEventRequest
{
    public string DeviceId { get; set; } = default!;

    public DateTime Timestamp { get; set; }

    public decimal Temperature { get; set; }

    public string Status { get; set; } = default!;

    public MetadataDto Metadata { get; set; } = default!;
}