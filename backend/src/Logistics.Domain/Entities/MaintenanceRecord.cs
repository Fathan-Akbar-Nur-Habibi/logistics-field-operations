
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class MaintenanceRecord : BaseEntity
{
    public Guid MaintenanceWorkOrderId { get; set; }

    public Guid? PerformedByEmployeeId { get; set; }

    public DateTimeOffset MaintenanceDate { get; set; }
        = DateTimeOffset.UtcNow;

    public string WorkDescription { get; set; } = string.Empty;

    public string? PartsReplaced { get; set; }

    public decimal? LaborCost { get; set; }

    public decimal? PartsCost { get; set; }

    public string? Notes { get; set; }

    // Navigation Properties
    public MaintenanceWorkOrder MaintenanceWorkOrder { get; set; }
        = null!;

    public Employee? PerformedByEmployee { get; set; }
}
