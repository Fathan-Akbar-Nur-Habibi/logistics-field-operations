
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class MaintenancePlan : BaseEntity
{
    public Guid AssetId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int IntervalDays { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? NextMaintenanceDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Asset Asset { get; set; } = null!;
}
