
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class MaterialRequest : BaseEntity
{
    public string RequestNumber { get; set; } = string.Empty;

    public Guid RequestingLocationId { get; set; }

    public Guid DestinationLocationId { get; set; }

    public DateTimeOffset RequestDate { get; set; }
        = DateTimeOffset.UtcNow;

    public DateOnly? RequiredDate { get; set; }

    public string Priority { get; set; } = "MEDIUM";

    public string Status { get; set; } = "PENDING";

    public string? Notes { get; set; }

    public Guid? RequestedByUserId { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    // Navigation Properties
    public Location RequestingLocation { get; set; } = null!;

    public Location DestinationLocation { get; set; } = null!;

    public ICollection<MaterialRequestItem> Items { get; set; }
        = new List<MaterialRequestItem>();

    public ICollection<Shipment> Shipments { get; set; }
        = new List<Shipment>();
}
