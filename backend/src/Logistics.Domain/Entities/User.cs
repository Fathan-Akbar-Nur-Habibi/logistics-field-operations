
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    // Simpan password dalam bentuk hash, bukan plaintext.
    public string PasswordHash { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset? LastLoginAt { get; set; }

    public int FailedLoginAttempts { get; set; } = 0;

    public DateTimeOffset? LockoutEnd { get; set; }

    // Navigation Properties
    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();

    public ICollection<AuditLog> AuditLogs { get; set; }
        = new List<AuditLog>();

    public ICollection<LoginHistory> LoginHistories { get; set; }
        = new List<LoginHistory>();
}
