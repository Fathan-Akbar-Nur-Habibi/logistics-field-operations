
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class StockTransaction : BaseEntity
{
    public string TransactionNumber { get; set; } = string.Empty;

    public Guid ItemId { get; set; }

    public Guid LocationId { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? Notes { get; set; }

    public Guid? PerformedByUserId { get; set; }

    public DateTimeOffset TransactionDate { get; set; }
        = DateTimeOffset.UtcNow;

    // Navigation Properties
    public Item Item { get; set; } = null!;

    public Location Location { get; set; } = null!;
}
