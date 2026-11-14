
using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Maintenance.Plan;

public class UpdateMaintenancePlanRequest
{
    [Required(ErrorMessage = "PlanName is required.")]
    [StringLength(150,MinimumLength = 3,
        ErrorMessage = "PlanName must be between 3 and 150 characters."
    )]
    public string PlanName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(
        1000,
        ErrorMessage = "Description cannot exceed 1000 characters."
    )]
    public string Description { get; set; } = string.Empty;

    [Range(1,3650,
        ErrorMessage = "IntervalDays must be between 1 and 3650."
    )]
    public int IntervalDays { get; set; }
    [Required(ErrorMessage = "StartDate is required.")]
    public DateOnly StartDate { get; set; }
    public bool IsActive { get; set; }
}
