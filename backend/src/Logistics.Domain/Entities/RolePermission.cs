
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    // Navigation Properties
    public Role Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;
}
