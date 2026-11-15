using Logistics.Domain.Entities;

namespace Logistics.Application.Interfaces.Crew;

public interface ICrewScheduleRepository
{
    Task<List<CrewSchedule>> GetCrewSchedulesAsync(DateOnly? workDate = null,
        Guid? employeeId = null,Guid? shiftId = null,Guid? locationId = null,
        string? status = null,CancellationToken cancellationToken = default);

    Task<CrewSchedule?> GetCrewScheduleByIdAsync(Guid id,CancellationToken cancellationToken = default);

    Task<CrewSchedule?> GetCrewScheduleForUpdateAsync(Guid id,CancellationToken cancellationToken = default);

    Task<bool> CrewScheduleExistsAsync(Guid employeeId,DateOnly workDate,
        Guid? excludeId = null,CancellationToken cancellationToken = default);

    Task<bool> IsEmployeeActiveAsync(Guid employeeId,CancellationToken cancellationToken = default);

    Task<bool> IsShiftActiveAsync(Guid shiftId,CancellationToken cancellationToken = default);

    Task<bool> IsLocationActiveAsync(Guid locationId,CancellationToken cancellationToken = default);

    Task AddCrewScheduleAsync(CrewSchedule crewSchedule,CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
