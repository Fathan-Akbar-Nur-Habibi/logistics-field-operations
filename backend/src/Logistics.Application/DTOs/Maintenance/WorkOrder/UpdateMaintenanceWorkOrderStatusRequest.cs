using System.ComponentModel.DataAnnotations;
namespace Logistics.Application.DTOs.Maintenance.WorkOrder;
public class UpdateMaintenanceWorkOrderStatusRequest
{
    [Required(ErrorMessage = "Status is required.")]
    [RegularExpression(
        "^(IN_PROGRESS|COMPLETED|CANCELLED)$",
        ErrorMessage = "Status must be IN_PROGRESS, COMPLETED, or CANCELLED."
    )]
    public string Status { get; set; } = string.Empty;
}