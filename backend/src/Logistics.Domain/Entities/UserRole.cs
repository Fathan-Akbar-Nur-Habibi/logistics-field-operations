
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTimeOffset AssignedAt { get; set; }
        = DateTimeOffset.UtcNow;

    // Navigation Properties
    public User User { get; set; } = null!;

    public Role Role { get; set; } = null!;
}
