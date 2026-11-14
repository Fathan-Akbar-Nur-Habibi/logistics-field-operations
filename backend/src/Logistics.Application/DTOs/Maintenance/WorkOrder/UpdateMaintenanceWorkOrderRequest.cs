using System.ComponentModel.DataAnnotations;
namespace Logistics.Application.DTOs.Maintenance.WorkOrder;

public class UpdateMaintenanceWorkOrderRequest
{
    [Required(ErrorMessage = "MaintenanceType is required.")]
    [RegularExpression(
        "^(PREVENTIVE|CORRECTIVE)$",
        ErrorMessage = "MaintenanceType must be PREVENTIVE or CORRECTIVE."
    )]
    public string MaintenanceType { get; set; } = "PREVENTIVE";

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200,
        MinimumLength = 3,
        ErrorMessage = "Title must be between 3 and 200 characters."
    )]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(2000,
        ErrorMessage = "Description cannot exceed 2000 characters."
    )]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "ScheduledDate is required.")]
    public DateOnly ScheduledDate { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    [RegularExpression(
        "^(LOW|MEDIUM|HIGH|CRITICAL)$",
        ErrorMessage = "Priority must be LOW, MEDIUM, HIGH, or CRITICAL."
    )]
    public string Priority { get; set; } = "MEDIUM";
    [Range(0,9999999999999999.99,
        ErrorMessage = "EstimatedCost cannot be negative."
    )]
    public decimal? EstimatedCost { get; set; }
    public Guid? AssignedToEmployeeId { get; set; }
}
