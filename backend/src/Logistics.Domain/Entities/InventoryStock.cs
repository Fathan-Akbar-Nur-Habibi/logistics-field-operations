
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class InventoryStock : BaseEntity
{
    public Guid ItemId { get; set; }

    public Guid LocationId { get; set; }

    public decimal Quantity { get; set; }

    public decimal ReservedQuantity { get; set; }

    // Navigation Properties
    public Item Item { get; set; } = null!;

    public Location Location { get; set; } = null!;

    // Quantity yang tersedia untuk digunakan
    public decimal AvailableQuantity => Quantity - ReservedQuantity;
}
