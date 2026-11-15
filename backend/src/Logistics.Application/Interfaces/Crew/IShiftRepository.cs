using Logistics.Domain.Entities;

namespace Logistics.Application.Interfaces.Crew;

public interface IShiftRepository
{
    Task<List<Shift>> GetShiftsAsync(string? search = null,
        bool? isActive = null,CancellationToken cancellationToken = default);

    Task<Shift?> GetShiftByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<Shift?> GetShiftForUpdateAsync(Guid id,CancellationToken cancellationToken = default);

    Task<bool> ShiftCodeExistsAsync(string code,Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddShiftAsync(Shift shift,CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}