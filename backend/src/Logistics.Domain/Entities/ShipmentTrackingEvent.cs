
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class ShipmentTrackingEvent : BaseEntity
{
    public Guid ShipmentId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? LocationName { get; set; }

    public DateTimeOffset EventDate { get; set; }
        = DateTimeOffset.UtcNow;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    // Navigation Properties
    public Shipment Shipment { get; set; } = null!;
}
