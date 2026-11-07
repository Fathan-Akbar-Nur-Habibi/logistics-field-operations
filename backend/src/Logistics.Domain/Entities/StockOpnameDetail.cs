
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class StockOpnameDetail : BaseEntity
{
    public Guid StockOpnameId { get; set; }

    public Guid ItemId { get; set; }

    public decimal SystemQuantity { get; set; }

    public decimal PhysicalQuantity { get; set; }

    public decimal Difference => PhysicalQuantity - SystemQuantity;

    public string? Notes { get; set; }

    // Navigation Properties
    public StockOpname StockOpname { get; set; } = null!;

    public Item Item { get; set; } = null!;
}
