using Logistics.Application.DTOs.Crew.CrewSchedule;

namespace Logistics.Application.Interfaces.Crew;

public interface ICrewScheduleService
{
    Task<List<CrewScheduleResponse>> GetCrewSchedulesAsync(DateOnly? workDate = null,
        Guid? employeeId = null,Guid? shiftId = null,Guid? locationId = null,
        string? status = null,CancellationToken cancellationToken = default);

    Task<CrewScheduleResponse?> GetCrewScheduleByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<CrewScheduleResponse> CreateCrewScheduleAsync(CreateCrewScheduleRequest request,
        CancellationToken cancellationToken = default);

    Task<CrewScheduleResponse?> UpdateCrewScheduleAsync(Guid id,UpdateCrewScheduleRequest request,
        CancellationToken cancellationToken = default);
}
