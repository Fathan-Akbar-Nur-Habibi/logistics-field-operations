namespace Logistics.Application.DTOs.Inventory;

public class InventoryStockResponse
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }

    public string ItemCode { get; set; } = string.Empty;

    public string ItemName { get; set; } = string.Empty;

    public Guid LocationId { get; set; }

    public string LocationCode { get; set; } = string.Empty;

    public string LocationName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal ReservedQuantity { get; set; }

    public decimal AvailableQuantity { get; set; }

    public decimal MinimumStock { get; set; }

    public bool IsBelowMinimumStock { get; set; }
}