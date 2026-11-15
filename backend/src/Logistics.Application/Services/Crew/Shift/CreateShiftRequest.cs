using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Crew.Shift;

public class CreateShiftRequest
{
    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
