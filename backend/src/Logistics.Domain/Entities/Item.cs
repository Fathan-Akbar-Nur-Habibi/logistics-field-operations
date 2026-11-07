
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Item : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid ItemCategoryId { get; set; }

    public Guid UnitOfMeasureId { get; set; }

    public decimal MinimumStock { get; set; }

    public decimal? UnitPrice { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ItemCategory ItemCategory { get; set; } = null!;

    public UnitOfMeasure UnitOfMeasure { get; set; } = null!;
}
