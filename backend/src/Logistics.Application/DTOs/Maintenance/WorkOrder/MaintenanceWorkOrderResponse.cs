namespace Logistics.Application.DTOs.Maintenance.WorkOrder;

public class MaintenanceWorkOrderResponse
{
    public Guid Id { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public Guid AssetId { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public Guid? MaintenancePlanId { get; set; }
    public string? MaintenancePlanName { get; set; }
    public string MaintenanceType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public Guid? AssignedToEmployeeId { get; set; }
    public string? AssignedToEmployeeName { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
