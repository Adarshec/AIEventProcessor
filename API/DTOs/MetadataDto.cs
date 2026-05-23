namespace API.DTOs;

public class MetadataDto
{
    public string ModelVersion { get; set; } = default!;

    public decimal Confidence { get; set; }
}