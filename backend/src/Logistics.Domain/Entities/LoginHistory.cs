
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class LoginHistory : BaseEntity
{
    public Guid? UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public bool IsSuccessful { get; set; }

    public string? FailureReason { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTimeOffset LoginAt { get; set; }
        = DateTimeOffset.UtcNow;

    // Navigation Properties
    public User? User { get; set; }
}
