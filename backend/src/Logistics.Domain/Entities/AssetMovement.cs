
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class AssetMovement : BaseEntity
{
    public Guid AssetId { get; set; }

    public Guid FromLocationId { get; set; }

    public Guid ToLocationId { get; set; }

    public DateTimeOffset MovementDate { get; set; }
        = DateTimeOffset.UtcNow;

    public string? Reason { get; set; }

    public Guid? MovedByUserId { get; set; }

    // Navigation Properties
    public Asset Asset { get; set; } = null!;

    public Location FromLocation { get; set; } = null!;

    public Location ToLocation { get; set; } = null!;
}
