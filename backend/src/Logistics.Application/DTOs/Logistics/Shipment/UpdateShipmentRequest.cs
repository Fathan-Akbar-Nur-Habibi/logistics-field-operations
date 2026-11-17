using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Logistics.Shipment;

public class UpdateShipmentRequest
{
    public Guid OriginLocationId { get; set; }

    public Guid DestinationLocationId { get; set; }

    [MaxLength(100)]
    public string? TrackingNumber { get; set; }

    [MaxLength(150)]
    public string? CarrierName { get; set; }

    public DateTimeOffset? ShippedAt { get; set; }

    public DateTimeOffset? EstimatedArrivalAt { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
