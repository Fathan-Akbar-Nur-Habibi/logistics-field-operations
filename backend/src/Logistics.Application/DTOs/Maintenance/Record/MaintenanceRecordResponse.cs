namespace Logistics.Application.DTOs.Maintenance.Record;

public class MaintenanceRecordResponse
{
    public Guid Id { get; set; }
    public Guid MaintenanceWorkOrderId { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string WorkOrderTitle { get; set; } = string.Empty;
    public Guid AssetId { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public Guid? PerformedByEmployeeId { get; set; }
    public string? PerformedByEmployeeName { get; set; }
    public DateTimeOffset MaintenanceDate { get; set; }
    public string WorkDescription { get; set; } = string.Empty;
    public string? PartsReplaced { get; set; }
    public decimal? LaborCost { get; set; }
    public decimal? PartsCost { get; set; }
    public decimal ActualCost { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
