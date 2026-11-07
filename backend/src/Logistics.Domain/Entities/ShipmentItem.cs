
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class ShipmentItem : BaseEntity
{
    public Guid ShipmentId { get; set; }

    public Guid ItemId { get; set; }

    public decimal Quantity { get; set; }

    public string? Notes { get; set; }

    // Navigation Properties
    public Shipment Shipment { get; set; } = null!;

    public Item Item { get; set; } = null!;
}
