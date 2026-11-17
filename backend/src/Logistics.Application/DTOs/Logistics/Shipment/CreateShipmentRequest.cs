using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Logistics.Shipment;

public class CreateShipmentRequest
{
    [Required]
    [MaxLength(50)]
    public string ShipmentNumber { get; set; } = string.Empty;

    public Guid? MaterialRequestId { get; set; }

    [Required]
    public Guid OriginLocationId { get; set; }

    [Required]
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
