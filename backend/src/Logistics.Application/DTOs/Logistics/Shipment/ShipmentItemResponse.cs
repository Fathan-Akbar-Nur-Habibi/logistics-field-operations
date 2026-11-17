namespace Logistics.Application.DTOs.Logistics.Shipment;

public class ShipmentItemResponse
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
