namespace Logistics.Application.DTOs.Inventory;

public class ItemResponse
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid ItemCategoryId { get; set; }

    public string ItemCategoryName { get; set; } = string.Empty;

    public Guid UnitOfMeasureId { get; set; }

    public string UnitOfMeasureName { get; set; } = string.Empty;

    public decimal MinimumStock { get; set; }

    public decimal? UnitPrice { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}