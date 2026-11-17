namespace Logistics.Application.DTOs.Logistics.Shipment;

public class ShipmentTrackingEventResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LocationName { get; set; }
    public DateTimeOffset EventDate { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
