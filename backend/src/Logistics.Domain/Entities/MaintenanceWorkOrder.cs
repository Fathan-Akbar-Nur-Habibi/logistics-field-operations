
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class MaintenanceWorkOrder : BaseEntity
{
    public string WorkOrderNumber { get; set; } = string.Empty;

    public Guid AssetId { get; set; }

    public Guid? MaintenancePlanId { get; set; }

    public string MaintenanceType { get; set; } = "PREVENTIVE";

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateOnly ScheduledDate { get; set; }

    public DateOnly? CompletedDate { get; set; }

    public string Priority { get; set; } = "MEDIUM";

    public string Status { get; set; } = "OPEN";

    public decimal? EstimatedCost { get; set; }

    public decimal? ActualCost { get; set; }

    public Guid? AssignedToEmployeeId { get; set; }

    public Guid? CreatedByUserId { get; set; }

    // Navigation Properties
    public Asset Asset { get; set; } = null!;

    public MaintenancePlan? MaintenancePlan { get; set; }

    public Employee? AssignedToEmployee { get; set; }

    public ICollection<MaintenanceRecord> Records { get; set; }
        = new List<MaintenanceRecord>();
}
