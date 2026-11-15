
using Logistics.Application.DTOs.Crew.Shift;

namespace Logistics.Application.Interfaces.Crew;

public interface IShiftService
{
    Task<List<ShiftResponse>> GetShiftsAsync(string? search = null,
        bool? isActive = null,CancellationToken cancellationToken = default);

    Task<ShiftResponse?> GetShiftByIdAsync(Guid id,CancellationToken cancellationToken = default);

    Task<ShiftResponse> CreateShiftAsync(CreateShiftRequest request,
        CancellationToken cancellationToken = default);

    Task<ShiftResponse> UpdateShiftAsync(Guid id,UpdateShiftRequest request,
        CancellationToken cancellationToken = default);
}
