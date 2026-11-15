using Logistics.Domain.Entities;

namespace Logistics.Application.Interfaces.Crew;

public interface IFieldAssignmentRepository
{
    Task<List<FieldAssignment>> GetFieldAssignmentsAsync(
        string? search = null,Guid? locationId = null,
        string? status = null,string? priority = null,
        DateTimeOffset? startDateFrom = null,DateTimeOffset? startDateTo = null,
        CancellationToken cancellationToken = default);

    Task<FieldAssignment?> GetFieldAssignmentByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<FieldAssignment?> GetFieldAssignmentForUpdateAsync(
        Guid id,CancellationToken cancellationToken = default);

    Task<bool> AssignmentNumberExistsAsync(string assignmentNumber,
        Guid? excludeId = null,CancellationToken cancellationToken = default);

    Task<bool> IsLocationActiveAsync(Guid locationId,CancellationToken cancellationToken = default);

    Task<bool> IsEmployeeActiveAsync(Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<bool> AssignmentMemberExistsAsync(Guid fieldAssignmentId,
        Guid employeeId,CancellationToken cancellationToken = default);

    Task<FieldAssignmentMember?> GetAssignmentMemberForUpdateAsync(
        Guid fieldAssignmentId,Guid memberId,
        CancellationToken cancellationToken = default);

    Task AddFieldAssignmentAsync(FieldAssignment fieldAssignment,
        CancellationToken cancellationToken = default);

    Task AddAssignmentMemberAsync(FieldAssignmentMember member,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
