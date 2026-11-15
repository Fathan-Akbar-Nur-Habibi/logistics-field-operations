namespace Logistics.Application.DTOs.Crew.FieldAssignment;

public class FieldAssignmentResponse
{
    public Guid Id { get; set; }
    public string AssignmentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? CreatedByUserId { get; set; }
    public List<FieldAssignmentMemberResponse> Members { get; set; }= new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
