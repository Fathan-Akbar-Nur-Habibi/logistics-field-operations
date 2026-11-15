using System.ComponentModel.DataAnnotations;
namespace Logistics.Application.DTOs.Crew;

public class UpdateEmployeeRequest
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Position { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool IsActive { get; set; } = true;
}