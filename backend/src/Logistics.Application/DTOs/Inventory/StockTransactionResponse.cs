namespace Logistics.Application.DTOs.Inventory;

public class StockTransactionResponse
{
    public Guid Id { get; set; }

    public string TransactionNumber { get; set; } = string.Empty;

    public Guid ItemId { get; set; }

    public string ItemCode { get; set; } = string.Empty;

    public string ItemName { get; set; } = string.Empty;

    public Guid LocationId { get; set; }

    public string LocationCode { get; set; } = string.Empty;

    public string LocationName { get; set; } = string.Empty;

    public string TransactionType { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? Notes { get; set; }

    public Guid? PerformedByUserId { get; set; }

    public DateTimeOffset TransactionDate { get; set; }
}