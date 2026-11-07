
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class CrewSchedule : BaseEntity
{
    public Guid EmployeeId { get; set; }

    public Guid ShiftId { get; set; }

    public Guid LocationId { get; set; }

    public DateOnly WorkDate { get; set; }

    public string Status { get; set; } = "SCHEDULED";

    public string? Notes { get; set; }

    // Navigation Properties
    public Employee Employee { get; set; } = null!;

    public Shift Shift { get; set; } = null!;

    public Location Location { get; set; } = null!;
}
