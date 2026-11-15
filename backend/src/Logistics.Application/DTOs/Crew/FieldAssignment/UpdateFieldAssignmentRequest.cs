using System.ComponentModel.DataAnnotations;
namespace Logistics.Application.DTOs.Crew.FieldAssignment;

public class UpdateFieldAssignmentRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [Required]
    public DateTimeOffset StartDate { get; set; }

    [Required]
    public DateTimeOffset EndDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Priority { get; set; } = "MEDIUM";
}
