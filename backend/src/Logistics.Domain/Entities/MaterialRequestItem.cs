
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class MaterialRequestItem : BaseEntity
{
    public Guid MaterialRequestId { get; set; }

    public Guid ItemId { get; set; }

    public decimal RequestedQuantity { get; set; }

    public decimal? ApprovedQuantity { get; set; }

    public string? Notes { get; set; }

    // Navigation Properties
    public MaterialRequest MaterialRequest { get; set; } = null!;

    public Item Item { get; set; } = null!;
}
