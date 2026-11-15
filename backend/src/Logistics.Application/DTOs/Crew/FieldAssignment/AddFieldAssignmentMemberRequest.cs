using System.ComponentModel.DataAnnotations;
namespace Logistics.Application.DTOs.Crew.FieldAssignment;

public class AddFieldAssignmentMemberRequest
{
    [Required]
    public Guid EmployeeId { get; set; }

    [MaxLength(200)]
    public string? Responsibility { get; set; }
}
