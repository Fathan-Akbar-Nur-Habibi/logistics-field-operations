using System.ComponentModel.DataAnnotations;
namespace Logistics.Application.DTOs.Maintenance.Record;

public class CreateMaintenanceRecordRequest
{
    [Required(ErrorMessage = "MaintenanceWorkOrderId is required.")]
    public Guid MaintenanceWorkOrderId { get; set; }

    public Guid? PerformedByEmployeeId { get; set; }

    [Required(ErrorMessage = "MaintenanceDate is required.")]
    public DateTimeOffset MaintenanceDate { get; set; }

    [Required(ErrorMessage = "WorkDescription is required.")]
    [StringLength(2000,
        MinimumLength = 3,
        ErrorMessage = "WorkDescription must be between 3 and 2000 characters."
    )]
    public string WorkDescription { get; set; } = string.Empty;

    [StringLength(1000,
        ErrorMessage = "PartsReplaced cannot exceed 1000 characters."
    )]
    public string? PartsReplaced { get; set; }

    [Range(0,9999999999999999.99,
        ErrorMessage = "LaborCost cannot be negative."
    )]
    public decimal? LaborCost { get; set; }

    [Range(0,9999999999999999.99,
        ErrorMessage = "PartsCost cannot be negative."
    )]
    public decimal? PartsCost { get; set; }

    [StringLength(1000,
        ErrorMessage = "Notes cannot exceed 1000 characters."
    )]
    public string? Notes { get; set; }
}
