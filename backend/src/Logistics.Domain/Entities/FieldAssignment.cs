
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class FieldAssignment : BaseEntity
{
    public string AssignmentNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid LocationId { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public string Priority { get; set; } = "MEDIUM";

    public string Status { get; set; } = "PLANNED";

    public Guid? CreatedByUserId { get; set; }

    // Navigation Properties
    public Location Location { get; set; } = null!;

    public ICollection<FieldAssignmentMember> Members { get; set; }
        = new List<FieldAssignmentMember>();
}
