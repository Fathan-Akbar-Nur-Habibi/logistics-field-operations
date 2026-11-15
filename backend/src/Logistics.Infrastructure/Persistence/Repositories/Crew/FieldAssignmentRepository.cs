using Logistics.Application.Interfaces.Crew;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Repositories.Crew;

public class FieldAssignmentRepository : IFieldAssignmentRepository
{
    private readonly LogisticsDbContext _context;

    public FieldAssignmentRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<List<FieldAssignment>> GetFieldAssignmentsAsync(
        string? search = null,
        Guid? locationId = null,
        string? status = null,
        string? priority = null,
        DateTimeOffset? startDateFrom = null,
        DateTimeOffset? startDateTo = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.FieldAssignments
            .AsNoTracking()
            .Include(x => x.Location)
            .Include(x => x.Members)
                .ThenInclude(x => x.Employee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.AssignmentNumber, $"%{keyword}%") ||
                EF.Functions.ILike(x.Title, $"%{keyword}%") ||
                (x.Description != null &&
                 EF.Functions.ILike(x.Description, $"%{keyword}%")) ||
                EF.Functions.ILike(x.Location.Name, $"%{keyword}%") ||
                x.Members.Any(m =>
                    EF.Functions.ILike(m.Employee.FullName, $"%{keyword}%") ||
                    EF.Functions.ILike(m.Employee.EmployeeNumber, $"%{keyword}%")));
        }

        if (locationId.HasValue)
        {
            query = query.Where(x => x.LocationId == locationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status.Trim());
        }

        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = query.Where(x => x.Priority == priority.Trim());
        }

        if (startDateFrom.HasValue)
        {
            var startDateFromUtc = startDateFrom.Value.ToUniversalTime();

            query = query.Where(x => x.StartDate >= startDateFromUtc);
        }

        if (startDateTo.HasValue)
        {
            var startDateToUtc = startDateTo.Value.ToUniversalTime();

            query = query.Where(x => x.StartDate <= startDateToUtc);
        }

        return await query.OrderBy(x => x.StartDate)
            .ThenBy(x => x.AssignmentNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<FieldAssignment?> GetFieldAssignmentByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FieldAssignments
            .AsNoTracking()
            .Include(x => x.Location)
            .Include(x => x.Members)
                .ThenInclude(x => x.Employee)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<FieldAssignment?> GetFieldAssignmentForUpdateAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FieldAssignments
            .Include(x => x.Location)
            .Include(x => x.Members)
                .ThenInclude(x => x.Employee)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> AssignmentNumberExistsAsync(string assignmentNumber,
        Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.FieldAssignments
            .AsNoTracking()
            .Where(x => x.AssignmentNumber == assignmentNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsLocationActiveAsync(Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == locationId && x.IsActive,
                cancellationToken);
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

    public async Task<bool> AssignmentMemberExistsAsync(Guid fieldAssignmentId, Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.FieldAssignmentMembers
            .AsNoTracking()
            .AnyAsync(
                x => x.FieldAssignmentId == fieldAssignmentId &&
                     x.EmployeeId == employeeId,
                cancellationToken);
    }

    public async Task<FieldAssignmentMember?> GetAssignmentMemberForUpdateAsync(
        Guid fieldAssignmentId, Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _context.FieldAssignmentMembers
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(
                x => x.FieldAssignmentId == fieldAssignmentId &&
                     x.Id == memberId,
                cancellationToken);
    }

    public async Task AddFieldAssignmentAsync(FieldAssignment fieldAssignment,
        CancellationToken cancellationToken = default)
    {
        await _context.FieldAssignments.AddAsync(
            fieldAssignment,
            cancellationToken);
    }

    public async Task AddAssignmentMemberAsync(FieldAssignmentMember member,
        CancellationToken cancellationToken = default)
    {
        await _context.FieldAssignmentMembers.AddAsync(member, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
