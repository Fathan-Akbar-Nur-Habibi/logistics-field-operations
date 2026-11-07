
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class FieldAssignmentMember : BaseEntity
{
    public Guid FieldAssignmentId { get; set; }

    public Guid EmployeeId { get; set; }

    public string? Responsibility { get; set; }

    public string Status { get; set; } = "ASSIGNED";

    public DateTimeOffset? CheckInTime { get; set; }

    public DateTimeOffset? CheckOutTime { get; set; }

    // Navigation Properties
    public FieldAssignment FieldAssignment { get; set; } = null!;

    public Employee Employee { get; set; } = null!;
}
