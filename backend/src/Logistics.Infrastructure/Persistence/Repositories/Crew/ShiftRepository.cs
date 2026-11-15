using Logistics.Application.Interfaces.Crew;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Repositories.Crew;

public class ShiftRepository : IShiftRepository
{
    private readonly LogisticsDbContext _context;

    public ShiftRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Shift>> GetShiftsAsync(string? search = null,
        bool? isActive = null,CancellationToken cancellationToken = default)
    {
        var query = _context.Shifts.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.Code, $"%{keyword}%") ||
                EF.Functions.ILike(x.Name, $"%{keyword}%"));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x =>
                x.IsActive == isActive.Value);
        }

        return await query.OrderBy(x => x.StartTime)
            .ThenBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<Shift?> GetShiftByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _context.Shifts.AsNoTracking().FirstOrDefaultAsync(
                x => x.Id == id,cancellationToken);
    }

    public async Task<Shift?> GetShiftForUpdateAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> ShiftCodeExistsAsync(string code,Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Shifts.AsNoTracking()
            .Where(x => x.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(x =>x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddShiftAsync(Shift shift,
        CancellationToken cancellationToken = default)
    {
        await _context.Shifts.AddAsync(shift,cancellationToken);
    }

    public async Task SaveChangesAsync( CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}