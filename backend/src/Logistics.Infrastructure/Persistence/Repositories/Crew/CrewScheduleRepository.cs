using Logistics.Application.Interfaces.Crew;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Repositories.Crew;

public class CrewScheduleRepository : ICrewScheduleRepository
{
    private readonly LogisticsDbContext _context;

    public CrewScheduleRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<List<CrewSchedule>> GetCrewSchedulesAsync(DateOnly? workDate = null,Guid? employeeId = null,
        Guid? shiftId = null,Guid? locationId = null, string? status = null,CancellationToken cancellationToken = default)
    {
        var query = _context.CrewSchedules
            .AsNoTracking()
            .Include(x => x.Employee)
            .Include(x => x.Shift)
            .Include(x => x.Location)
            .AsQueryable();

        if (workDate.HasValue)
        {
            query = query.Where(x => x.WorkDate == workDate.Value);
        }

        if (employeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == employeeId.Value);
        }

        if (shiftId.HasValue)
        {
            query = query.Where(x => x.ShiftId == shiftId.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(x => x.LocationId == locationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status.Trim());
        }

        return await query
            .OrderBy(x => x.WorkDate)
            .ThenBy(x => x.Employee.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<CrewSchedule?> GetCrewScheduleByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _context.CrewSchedules
            .AsNoTracking()
            .Include(x => x.Employee)
            .Include(x => x.Shift)
            .Include(x => x.Location)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<CrewSchedule?> GetCrewScheduleForUpdateAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _context.CrewSchedules
            .Include(x => x.Employee)
            .Include(x => x.Shift)
            .Include(x => x.Location)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> CrewScheduleExistsAsync(Guid employeeId,DateOnly workDate,
        Guid? excludeId = null,CancellationToken cancellationToken = default)
    {
        var query = _context.CrewSchedules
            .AsNoTracking()
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.WorkDate == workDate);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmployeeActiveAsync(Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == employeeId && x.IsActive,
                cancellationToken);
    }

    public async Task<bool> IsShiftActiveAsync(Guid shiftId,CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == shiftId && x.IsActive,
                cancellationToken);
    }

    public async Task<bool> IsLocationActiveAsync(Guid locationId,CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == locationId && x.IsActive,
                cancellationToken);
    }

    public async Task AddCrewScheduleAsync(CrewSchedule crewSchedule,CancellationToken cancellationToken = default)
    {
        await _context.CrewSchedules.AddAsync(crewSchedule,cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
