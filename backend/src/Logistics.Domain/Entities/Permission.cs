
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Permission : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}
