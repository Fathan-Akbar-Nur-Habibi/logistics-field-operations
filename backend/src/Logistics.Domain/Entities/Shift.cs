
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Shift : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<CrewSchedule> CrewSchedules { get; set; }
        = new List<CrewSchedule>();
}
