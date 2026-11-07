
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class StockOpname : BaseEntity
{
    public string OpnameNumber { get; set; } = string.Empty;

    public Guid LocationId { get; set; }

    public DateTimeOffset OpnameDate { get; set; }
        = DateTimeOffset.UtcNow;

    public string Status { get; set; } = "DRAFT";

    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    // Navigation Properties
    public Location Location { get; set; } = null!;

    public ICollection<StockOpnameDetail> Details { get; set; }
        = new List<StockOpnameDetail>();
}
