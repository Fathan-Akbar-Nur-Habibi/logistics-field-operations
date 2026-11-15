using System.ComponentModel.DataAnnotations;
namespace Logistics.Application.DTOs.Crew.FieldAssignment;

public class UpdateFieldAssignmentStatusRequest
{
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = string.Empty;
}
