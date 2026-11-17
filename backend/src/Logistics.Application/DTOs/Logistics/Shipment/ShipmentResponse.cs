namespace Logistics.Application.DTOs.Logistics.Shipment;

public class ShipmentResponse
{
    public Guid Id { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public Guid? MaterialRequestId { get; set; }
    public Guid OriginLocationId { get; set; }
    public string OriginLocationCode { get; set; } = string.Empty;
    public string OriginLocationName { get; set; } = string.Empty;
    public Guid DestinationLocationId { get; set; }
    public string DestinationLocationCode { get; set; } = string.Empty;
    public string DestinationLocationName { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    public string? CarrierName { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }
    public DateTimeOffset? EstimatedArrivalAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public List<ShipmentItemResponse> Items { get; set; }= new();
    public List<ShipmentTrackingEventResponse> TrackingEvents { get; set; }= new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
