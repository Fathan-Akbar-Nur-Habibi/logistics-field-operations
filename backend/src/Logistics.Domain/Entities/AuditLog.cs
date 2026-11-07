
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    // Simpan snapshot data dalam format JSON.
    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTimeOffset OccurredAt { get; set; }
        = DateTimeOffset.UtcNow;

    // Navigation Properties
    public User? User { get; set; }
}
