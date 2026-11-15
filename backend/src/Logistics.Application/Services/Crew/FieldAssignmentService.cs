using Logistics.Application.DTOs.Crew.FieldAssignment;
using Logistics.Application.Interfaces.Crew;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services.Crew;

public class FieldAssignmentService : IFieldAssignmentService
{
    private readonly IFieldAssignmentRepository _repository;

    private static readonly HashSet<string> AllowedPriorities =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "LOW",
            "MEDIUM",
            "HIGH",
            "URGENT"
        };

    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "PLANNED",
            "IN_PROGRESS",
            "COMPLETED",
            "CANCELLED"
        };

    public FieldAssignmentService( IFieldAssignmentRepository repository)
    {
        _repository = repository;
    }

    // GET ALL
    public async Task<List<FieldAssignmentResponse>> GetFieldAssignmentsAsync(
        string? search = null, Guid? locationId = null, string? status = null,
        string? priority = null, DateTimeOffset? startDateFrom = null,
        DateTimeOffset? startDateTo = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(status))
        {
            status = NormalizeValue(status);

            if (!AllowedStatuses.Contains(status))
            {
                throw new ArgumentException("Invalid assignment status.");
            }
        }

        if (!string.IsNullOrWhiteSpace(priority))
        {
            priority = NormalizeValue(priority);

            if (!AllowedPriorities.Contains(priority))
            {
                throw new ArgumentException("Invalid assignment priority.");
            }
        }

        if (startDateFrom.HasValue &&
            startDateTo.HasValue &&
            startDateTo.Value < startDateFrom.Value)
        {
            throw new ArgumentException(
                "StartDateTo cannot be earlier than StartDateFrom.");
        }

        var assignments = await _repository.GetFieldAssignmentsAsync(
            search,
            locationId,
            status,
            priority,
            startDateFrom,
            startDateTo,
            cancellationToken);

        return assignments.Select(MapToResponse).ToList();
    }

    // GET BY ID
    public async Task<FieldAssignmentResponse?> GetFieldAssignmentByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Assignment ID is required.");
        }

        var assignment = await _repository.GetFieldAssignmentByIdAsync(id,cancellationToken);
        return assignment == null ? null: MapToResponse(assignment);
    }

    // CREATE
    public async Task<FieldAssignmentResponse> CreateFieldAssignmentAsync(CreateFieldAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateAssignmentRequest(
            request.AssignmentNumber,
            request.Title,
            request.LocationId,
            request.StartDate,
            request.EndDate,
            request.Priority,
            request.Description);

        var assignmentNumber = request.AssignmentNumber.Trim().ToUpperInvariant();
        var priority = NormalizeValue(request.Priority);

        if (!AllowedPriorities.Contains(priority))
        {
            throw new ArgumentException(
                "Invalid priority. Allowed values: LOW, MEDIUM, HIGH, URGENT.");
        }

        var isDuplicate = await _repository.AssignmentNumberExistsAsync(
                assignmentNumber,cancellationToken: cancellationToken);

        if (isDuplicate)
        {
            throw new InvalidOperationException(
                "Assignment number already exists.");
        }

        await ValidateLocationAsync( request.LocationId,cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var assignment = new FieldAssignment
        {
            Id = Guid.NewGuid(),
            AssignmentNumber = assignmentNumber,
            Title = request.Title.Trim(),
            Description = NormalizeOptionalText(request.Description),
            LocationId = request.LocationId,
            StartDate = request.StartDate.ToUniversalTime(),
            EndDate = request.EndDate.ToUniversalTime(),
            Priority = priority,
            Status = "PLANNED",
            CreatedByUserId = null,
            CreatedAt = now
        };

        await _repository.AddFieldAssignmentAsync(assignment,cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        var createdAssignment = await _repository.GetFieldAssignmentByIdAsync(
                assignment.Id, cancellationToken);

        if (createdAssignment == null)
        {
            throw new InvalidOperationException(
                "Assignment was created but could not be retrieved.");
        }

        return MapToResponse(createdAssignment);
    }

    // UPDATE
    public async Task<FieldAssignmentResponse?> UpdateFieldAssignmentAsync(
        Guid id,
        UpdateFieldAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Assignment ID is required.");
        }

        ValidateAssignmentRequest("VALID",request.Title,request.LocationId,
            request.StartDate, request.EndDate,
            request.Priority,
            request.Description);

        var priority = NormalizeValue(request.Priority);

        if (!AllowedPriorities.Contains(priority))
        {
            throw new ArgumentException(
                "Invalid priority. Allowed values: LOW, MEDIUM, HIGH, URGENT.");
        }

        var assignment =
            await _repository.GetFieldAssignmentForUpdateAsync(
                id, cancellationToken);

        if (assignment == null)
        {
            return null;
        }

        if (assignment.Status == "COMPLETED" ||
            assignment.Status == "CANCELLED")
        {
            throw new InvalidOperationException(
                "Cannot update a completed or cancelled assignment.");
        }

        await ValidateLocationAsync( request.LocationId,cancellationToken);

        assignment.Title = request.Title.Trim();
        assignment.Description =
            NormalizeOptionalText(request.Description);
        assignment.LocationId = request.LocationId;
        assignment.StartDate = request.StartDate.ToUniversalTime();
        assignment.EndDate = request.EndDate.ToUniversalTime();
        assignment.Priority = priority;
        assignment.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedAssignment =
            await _repository.GetFieldAssignmentByIdAsync(
                id,
                cancellationToken);

        if (updatedAssignment == null)
        {
            throw new InvalidOperationException(
                "Assignment was updated but could not be retrieved.");
        }

        return MapToResponse(updatedAssignment);
    }

    // UPDATE STATUS
    public async Task<FieldAssignmentResponse?> UpdateStatusAsync(
        Guid id,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Assignment ID is required.");
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Status is required.");
        }

        var normalizedStatus = NormalizeValue(status);

        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            throw new ArgumentException(
                "Invalid assignment status.");
        }

        var assignment =
            await _repository.GetFieldAssignmentForUpdateAsync(
                id,
                cancellationToken);

        if (assignment == null)
        {
            return null;
        }

        ValidateStatusTransition(assignment.Status,normalizedStatus);

        assignment.Status = normalizedStatus;
        assignment.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedAssignment =
            await _repository.GetFieldAssignmentByIdAsync(
                id, cancellationToken);

        return updatedAssignment == null
            ? throw new InvalidOperationException(
                "Updated assignment could not be retrieved.")
            : MapToResponse(updatedAssignment);
    }

    // ADD MEMBER
    public async Task<FieldAssignmentMemberResponse> AddMemberAsync(
        Guid assignmentId, AddFieldAssignmentMemberRequest request, CancellationToken cancellationToken = default)
    {
        if (assignmentId == Guid.Empty)
        {
            throw new ArgumentException("Assignment ID is required.");
        }

        if (request.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.");
        }

        if (request.Responsibility?.Length > 200)
        {
            throw new ArgumentException(
                "Responsibility cannot exceed 200 characters.");
        }

        var assignment =
            await _repository.GetFieldAssignmentByIdAsync(
                assignmentId,
                cancellationToken);

        if (assignment == null)
        {
            throw new KeyNotFoundException("Assignment not found.");
        }

        if (assignment.Status == "COMPLETED" ||
            assignment.Status == "CANCELLED")
        {
            throw new InvalidOperationException(
                "Cannot add members to a completed or cancelled assignment.");
        }

        var isEmployeeActive =
            await _repository.IsEmployeeActiveAsync(
                request.EmployeeId,
                cancellationToken);

        if (!isEmployeeActive)
        {
            throw new InvalidOperationException(
                "Employee does not exist or is inactive.");
        }

        var isDuplicate =
            await _repository.AssignmentMemberExistsAsync(
                assignmentId, request.EmployeeId,
                cancellationToken);

        if (isDuplicate)
        {
            throw new InvalidOperationException(
                "Employee is already assigned to this assignment.");
        }

        var member = new FieldAssignmentMember
        {
            Id = Guid.NewGuid(),
            FieldAssignmentId = assignmentId,
            EmployeeId = request.EmployeeId,
            Responsibility = NormalizeOptionalText(request.Responsibility),
            Status = "ASSIGNED",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddAssignmentMemberAsync(
            member, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        var createdMember = await _repository.GetAssignmentMemberForUpdateAsync(
                assignmentId, member.Id, cancellationToken);

        if (createdMember == null)
        {
            throw new InvalidOperationException(
                "Assignment member was created but could not be retrieved.");
        }

        return MapMemberToResponse(createdMember);
    }

    // CHECK IN
    public async Task<FieldAssignmentMemberResponse> CheckInMemberAsync(
        Guid assignmentId,Guid memberId,
        CancellationToken cancellationToken = default)
    {
        if (assignmentId == Guid.Empty || memberId == Guid.Empty)
        {
            throw new ArgumentException(
                "Assignment ID and Member ID are required.");
        }

        var assignment =
            await _repository.GetFieldAssignmentByIdAsync(assignmentId,
                cancellationToken);

        if (assignment == null)
        {
            throw new KeyNotFoundException("Assignment not found.");
        }

        if (assignment.Status != "IN_PROGRESS")
        {
            throw new InvalidOperationException(
                "Check-in is only allowed when assignment is IN_PROGRESS.");
        }

        var member =
            await _repository.GetAssignmentMemberForUpdateAsync(
                assignmentId,memberId,cancellationToken);

        if (member == null)
        {
            throw new KeyNotFoundException(
                "Assignment member not found.");
        }

        if (member.Status != "ASSIGNED")
        {
            throw new InvalidOperationException(
                "Member has already checked in or completed check-out.");
        }

        member.CheckInTime = DateTimeOffset.UtcNow;
        member.Status = "CHECKED_IN";
        member.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return MapMemberToResponse(member);
    }

    // CHECK OUT
    public async Task<FieldAssignmentMemberResponse> CheckOutMemberAsync(
        Guid assignmentId,Guid memberId,
        CancellationToken cancellationToken = default)
    {
        if (assignmentId == Guid.Empty || memberId == Guid.Empty)
        {
            throw new ArgumentException(
                "Assignment ID and Member ID are required.");
        }

        var member =
            await _repository.GetAssignmentMemberForUpdateAsync(
                assignmentId,
                memberId,
                cancellationToken);

        if (member == null)
        {
            throw new KeyNotFoundException(
                "Assignment member not found.");
        }

        if (member.Status != "CHECKED_IN" ||
            !member.CheckInTime.HasValue)
        {
            throw new InvalidOperationException(
                "Member must check in before checking out.");
        }

        var checkOutTime = DateTimeOffset.UtcNow;

        if (checkOutTime < member.CheckInTime.Value)
        {
            throw new InvalidOperationException(
                "Check-out time cannot be earlier than check-in time.");
        }

        member.CheckOutTime = checkOutTime;
        member.Status = "CHECKED_OUT";
        member.UpdatedAt = checkOutTime;

        await _repository.SaveChangesAsync(cancellationToken);
        return MapMemberToResponse(member);
    }

    // VALIDATION HELPERS

    private async Task ValidateLocationAsync(Guid locationId,
        CancellationToken cancellationToken)
    {
        if (locationId == Guid.Empty)
        {
            throw new ArgumentException("LocationId is required.");
        }

        var isActive = await _repository.IsLocationActiveAsync(
            locationId,cancellationToken);

        if (!isActive)
        {
            throw new InvalidOperationException(
                "Location does not exist or is inactive.");
        }
    }

    private static void ValidateAssignmentRequest(string assignmentNumber,
        string title, Guid locationId,DateTimeOffset startDate,
        DateTimeOffset endDate,string priority,string? description)
    {
        if (string.IsNullOrWhiteSpace(assignmentNumber))
        {
            throw new ArgumentException(
                "AssignmentNumber is required.");
        }

        if (assignmentNumber.Trim().Length > 50)
        {
            throw new ArgumentException(
                "AssignmentNumber cannot exceed 50 characters.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (title.Trim().Length > 200)
        {
            throw new ArgumentException(
                "Title cannot exceed 200 characters.");
        }

        if (locationId == Guid.Empty)
        {
            throw new ArgumentException("LocationId is required.");
        }

        if (startDate == default || endDate == default)
        {
            throw new ArgumentException(
                "StartDate and EndDate are required.");
        }

        if (endDate <= startDate)
        {
            throw new ArgumentException(
                "EndDate must be later than StartDate.");
        }

        if (string.IsNullOrWhiteSpace(priority))
        {
            throw new ArgumentException("Priority is required.");
        }

        if (description?.Length > 2000)
        {
            throw new ArgumentException(
                "Description cannot exceed 2000 characters.");
        }
    }

    private static void ValidateStatusTransition(string currentStatus,string newStatus)
    {
        var isAllowed =
            currentStatus switch
            {
                "PLANNED" =>
                    newStatus == "IN_PROGRESS" ||
                    newStatus == "CANCELLED",

                "IN_PROGRESS" =>
                    newStatus == "COMPLETED" ||
                    newStatus == "CANCELLED",

                _ => false
            };

        if (!isAllowed)
        {
            throw new InvalidOperationException(
                $"Status cannot be changed from {currentStatus} to {newStatus}.");
        }
    }

    private static string NormalizeValue(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null: value.Trim();
    }

    // RESPONSE MAPPING
    private static FieldAssignmentResponse MapToResponse(FieldAssignment assignment)
    {
        return new FieldAssignmentResponse
        {
            Id = assignment.Id,
            AssignmentNumber = assignment.AssignmentNumber,
            Title = assignment.Title,
            Description = assignment.Description,

            LocationId = assignment.LocationId,
            LocationCode = assignment.Location.Code,
            LocationName = assignment.Location.Name,

            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,

            Priority = assignment.Priority,
            Status = assignment.Status,
            CreatedByUserId = assignment.CreatedByUserId,

            Members = assignment.Members
                .Select(MapMemberToResponse)
                .ToList(),

            CreatedAt = assignment.CreatedAt,
            UpdatedAt = assignment.UpdatedAt
        };
    }

    private static FieldAssignmentMemberResponse MapMemberToResponse(
        FieldAssignmentMember member)
    {
        return new FieldAssignmentMemberResponse
        {
            Id = member.Id,

            EmployeeId = member.EmployeeId,
            EmployeeNumber = member.Employee.EmployeeNumber,
            EmployeeName = member.Employee.FullName,

            Responsibility = member.Responsibility,
            Status = member.Status,

            CheckInTime = member.CheckInTime,
            CheckOutTime = member.CheckOutTime,

            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt
        };
    }
}
