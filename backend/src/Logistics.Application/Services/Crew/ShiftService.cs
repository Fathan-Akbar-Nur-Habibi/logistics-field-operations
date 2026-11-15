using Logistics.Application.DTOs.Crew.Shift;
using Logistics.Application.Interfaces.Crew;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services.Crew;

public class ShiftService : IShiftService
{
    private readonly IShiftRepository _repository;

    public ShiftService(IShiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ShiftResponse>> GetShiftsAsync(string? search = null,
        bool? isActive = null,CancellationToken cancellationToken = default)
    {
        var shifts = await _repository.GetShiftsAsync(search,
            isActive,cancellationToken);

        return shifts.Select(MapToResponse).ToList();
    }

    public async Task<ShiftResponse?> GetShiftByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Shift ID tidak valid.");
        }

        var shift = await _repository.GetShiftByIdAsync(id,cancellationToken);

        return shift == null ? null : MapToResponse(shift);
    }

    public async Task<ShiftResponse> CreateShiftAsync(CreateShiftRequest request,
     CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var code = NormalizeRequired(request.Code,
            "Shift Code required field.");

        var name = NormalizeRequired(
            request.Name,"Shift Name required field.");

        ValidateWorkingHours(request.StartTime,request.EndTime);

        if (await _repository.ShiftCodeExistsAsync(code,cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException($"Shift Code '{code}' has been used.");
        }

        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Description = NormalizeOptional(request.Description),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddShiftAsync(shift,cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return MapToResponse(shift);
    }

    public async Task<ShiftResponse> UpdateShiftAsync(Guid id,
        UpdateShiftRequest request,CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id == Guid.Empty)
        {
            throw new ArgumentException("Shift ID tidak valid.");
        }

        var shift = await _repository.GetShiftForUpdateAsync(id,cancellationToken);

        if (shift == null)
        {
            throw new KeyNotFoundException(
                $"Shift with ID '{id}' Not Found.");
        }

        var name = NormalizeRequired(
            request.Name,
            "Shift Name Required field.");

        ValidateWorkingHours(request.StartTime,request.EndTime);

        shift.Name = name;
        shift.StartTime = request.StartTime;
        shift.EndTime = request.EndTime;
        shift.Description = NormalizeOptional(request.Description);
        shift.IsActive = request.IsActive;
        shift.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return MapToResponse(shift);
    }

    private static void ValidateWorkingHours(TimeOnly startTime,TimeOnly endTime)
    {
        if (startTime == endTime)
        {
            throw new ArgumentException(
                "The start time and end time must not be the same.");
        }

        // EndTime boleh lebih kecil dari StartTime.
        // Contoh shift malam: 22:00 sampai 06:00.
    }

    private static string NormalizeRequired(string? value,string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(errorMessage);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null: value.Trim();
    }

    private static ShiftResponse MapToResponse(Shift shift)
    {
        return new ShiftResponse
        {
            Id = shift.Id,
            Code = shift.Code,
            Name = shift.Name,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            Description = shift.Description,
            IsActive = shift.IsActive,
            CreatedAt = shift.CreatedAt,
            UpdatedAt = shift.UpdatedAt
        };
    }
}
