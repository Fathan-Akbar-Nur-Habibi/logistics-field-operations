using Logistics.Application.DTOs.Crew.FieldAssignment;

namespace Logistics.Application.Interfaces.Crew;

public interface IFieldAssignmentService
{
    Task<List<FieldAssignmentResponse>> GetFieldAssignmentsAsync(
        string? search = null,
        Guid? locationId = null,
        string? status = null,
        string? priority = null,
        DateTimeOffset? startDateFrom = null,
        DateTimeOffset? startDateTo = null,
        CancellationToken cancellationToken = default);

    Task<FieldAssignmentResponse?> GetFieldAssignmentByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<FieldAssignmentResponse> CreateFieldAssignmentAsync(
        CreateFieldAssignmentRequest request,CancellationToken cancellationToken = default);

    Task<FieldAssignmentResponse?> UpdateFieldAssignmentAsync(
        Guid id,UpdateFieldAssignmentRequest request,
        CancellationToken cancellationToken = default);

    Task<FieldAssignmentResponse?> UpdateStatusAsync(Guid id,
        string status,CancellationToken cancellationToken = default);

    Task<FieldAssignmentMemberResponse> AddMemberAsync(Guid assignmentId,AddFieldAssignmentMemberRequest request,
        CancellationToken cancellationToken = default);

    Task<FieldAssignmentMemberResponse> CheckInMemberAsync(Guid assignmentId,
        Guid memberId,CancellationToken cancellationToken = default);

    Task<FieldAssignmentMemberResponse> CheckOutMemberAsync(Guid assignmentId,
        Guid memberId,CancellationToken cancellationToken = default);
}
