
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Employee : BaseEntity
{
    public string EmployeeNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Position { get; set; }

    public string? Department { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool IsActive { get; set; } = true;
}
