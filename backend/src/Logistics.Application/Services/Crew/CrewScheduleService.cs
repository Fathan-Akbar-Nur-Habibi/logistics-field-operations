using Logistics.Application.DTOs.Crew.CrewSchedule;
using Logistics.Application.Interfaces.Crew;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services.Crew;

public class CrewScheduleService : ICrewScheduleService
{
    private readonly ICrewScheduleRepository _repository;

    private static readonly HashSet<string> AllowedStatuses =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "SCHEDULED",
            "CONFIRMED",
            "IN_PROGRESS",
            "COMPLETED",
            "CANCELLED"
        };

    public CrewScheduleService(ICrewScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CrewScheduleResponse>> GetCrewSchedulesAsync(DateOnly? workDate = null,
        Guid? employeeId = null, Guid? shiftId = null,
        Guid? locationId = null, string? status = null,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(status))
        {
            status = NormalizeStatus(status);

            if (!AllowedStatuses.Contains(status))
            {
                throw new ArgumentException("Invalid crew schedule status.");
            }
        }

        var schedules = await _repository.GetCrewSchedulesAsync(
            workDate,
            employeeId,
            shiftId,
            locationId,
            status,
            cancellationToken);

        return schedules.Select(MapToResponse).ToList();
    }

    public async Task<CrewScheduleResponse?> GetCrewScheduleByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Crew schedule ID is required.");
        }

        var schedule = await _repository.GetCrewScheduleByIdAsync(id,cancellationToken);

        return schedule == null ? null : MapToResponse(schedule);
    }

    public async Task<CrewScheduleResponse> CreateCrewScheduleAsync(CreateCrewScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(
            request.EmployeeId,
            request.ShiftId,
            request.LocationId,
            request.WorkDate,
            request.Notes);

        await ValidateActiveReferencesAsync(
            request.EmployeeId,
            request.ShiftId,
            request.LocationId,
            cancellationToken);

        var isDuplicate = await _repository.CrewScheduleExistsAsync(
            request.EmployeeId,
            request.WorkDate,
            cancellationToken: cancellationToken);

        if (isDuplicate)
        {
            throw new InvalidOperationException(
                "This employee already has a schedule on the selected work date.");
        }

        var now = DateTimeOffset.UtcNow;

        var schedule = new CrewSchedule
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            ShiftId = request.ShiftId,
            LocationId = request.LocationId,
            WorkDate = request.WorkDate,
            Status = "SCHEDULED",
            Notes = NormalizeOptionalText(request.Notes),
            CreatedAt = now
        };

        await _repository.AddCrewScheduleAsync(schedule,cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        var createdSchedule = await _repository.GetCrewScheduleByIdAsync(
            schedule.Id,
            cancellationToken);

        if (createdSchedule == null)
        {
            throw new InvalidOperationException(
                "Crew schedule was created but could not be retrieved.");
        }

        return MapToResponse(createdSchedule);
    }

    public async Task<CrewScheduleResponse?> UpdateCrewScheduleAsync(Guid id,
        UpdateCrewScheduleRequest request,CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Crew schedule ID is required.");
        }

        ValidateRequest(
            request.EmployeeId,
            request.ShiftId,
            request.LocationId,
            request.WorkDate,
            request.Notes);

        var normalizedStatus = NormalizeStatus(request.Status);

        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            throw new ArgumentException(
                "Invalid crew schedule status. Allowed values: " +
                string.Join(", ", AllowedStatuses));
        }

        var schedule = await _repository.GetCrewScheduleForUpdateAsync(
            id,
            cancellationToken);

        if (schedule == null)
        {
            return null;
        }

        await ValidateActiveReferencesAsync(
            request.EmployeeId,
            request.ShiftId,
            request.LocationId,
            cancellationToken);

        var isDuplicate = await _repository.CrewScheduleExistsAsync(
            request.EmployeeId,
            request.WorkDate,
            excludeId: id,
            cancellationToken: cancellationToken);

        if (isDuplicate)
        {
            throw new InvalidOperationException(
                "This employee already has another schedule on the selected work date.");
        }

        schedule.EmployeeId = request.EmployeeId;
        schedule.ShiftId = request.ShiftId;
        schedule.LocationId = request.LocationId;
        schedule.WorkDate = request.WorkDate;
        schedule.Status = normalizedStatus;
        schedule.Notes = NormalizeOptionalText(request.Notes);
        schedule.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedSchedule = await _repository.GetCrewScheduleByIdAsync(id,cancellationToken);

        if (updatedSchedule == null)
        {
            throw new InvalidOperationException(
                "Crew schedule was updated but could not be retrieved.");
        }

        return MapToResponse(updatedSchedule);
    }

    private async Task ValidateActiveReferencesAsync(Guid employeeId,Guid shiftId,
        Guid locationId,CancellationToken cancellationToken)
    {
        if (!await _repository.IsEmployeeActiveAsync(employeeId,cancellationToken))
        {
            throw new InvalidOperationException(
                "Employee does not exist or is inactive.");
        }

        if (!await _repository.IsShiftActiveAsync(shiftId,cancellationToken))
        {
            throw new InvalidOperationException(
                "Shift does not exist or is inactive.");
        }

        if (!await _repository.IsLocationActiveAsync(locationId,cancellationToken))
        {
            throw new InvalidOperationException(
                "Location does not exist or is inactive.");
        }
    }

    private static void ValidateRequest(Guid employeeId,Guid shiftId,Guid locationId,
        DateOnly workDate,string? notes)
    {
        if (employeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.");
        }

        if (shiftId == Guid.Empty)
        {
            throw new ArgumentException("ShiftId is required.");
        }

        if (locationId == Guid.Empty)
        {
            throw new ArgumentException("LocationId is required.");
        }

        if (workDate == DateOnly.MinValue)
        {
            throw new ArgumentException("WorkDate is required.");
        }

        if (notes?.Length > 1000)
        {
            throw new ArgumentException(
                "Notes cannot exceed 1000 characters.");
        }
    }

    private static string NormalizeStatus(string status)
    {
        return status.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null: value.Trim();
    }

    private static CrewScheduleResponse MapToResponse(CrewSchedule schedule)
    {
        return new CrewScheduleResponse
        {
            Id = schedule.Id,

            EmployeeId = schedule.EmployeeId,
            EmployeeNumber = schedule.Employee.EmployeeNumber,
            EmployeeName = schedule.Employee.FullName,

            ShiftId = schedule.ShiftId,
            ShiftCode = schedule.Shift.Code,
            ShiftName = schedule.Shift.Name,
            StartTime = schedule.Shift.StartTime,
            EndTime = schedule.Shift.EndTime,

            LocationId = schedule.LocationId,
            LocationCode = schedule.Location.Code,
            LocationName = schedule.Location.Name,

            WorkDate = schedule.WorkDate,
            Status = schedule.Status,
            Notes = schedule.Notes,

            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt
        };
    }
}
