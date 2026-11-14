namespace Logistics.Application.DTOs.Maintenance.Plan;

public class MaintenancePlanResponse
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int IntervalDays { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? NextMaintenanceDate { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
