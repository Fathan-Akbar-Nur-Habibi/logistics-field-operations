
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Shipment : BaseEntity
{
    public string ShipmentNumber { get; set; } = string.Empty;

    public Guid? MaterialRequestId { get; set; }

    public Guid OriginLocationId { get; set; }

    public Guid DestinationLocationId { get; set; }

    public string? TrackingNumber { get; set; }

    public string? CarrierName { get; set; }

    public DateTimeOffset? ShippedAt { get; set; }

    public DateTimeOffset? EstimatedArrivalAt { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }

    public string Status { get; set; } = "PREPARING";

    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }

    // Navigation Properties
    public MaterialRequest? MaterialRequest { get; set; }

    public Location OriginLocation { get; set; } = null!;

    public Location DestinationLocation { get; set; } = null!;

    public ICollection<ShipmentItem> Items { get; set; }
        = new List<ShipmentItem>();

    public ICollection<ShipmentTrackingEvent> TrackingEvents { get; set; }
        = new List<ShipmentTrackingEvent>();
}
