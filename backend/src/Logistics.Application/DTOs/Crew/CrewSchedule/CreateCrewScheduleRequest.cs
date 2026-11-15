using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Crew.CrewSchedule;

public class CreateCrewScheduleRequest
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public Guid ShiftId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [Required]
    public DateOnly WorkDate { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
