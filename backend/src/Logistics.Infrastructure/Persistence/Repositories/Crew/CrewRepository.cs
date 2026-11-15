using Logistics.Application.Interfaces.Crew;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Repositories.Crew;

public class CrewRepository : ICrewRepository
{
    private readonly LogisticsDbContext _context;

    public CrewRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetEmployeesAsync(string? search = null,
        string? department = null, string? position = null,bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Employees
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.EmployeeNumber, $"%{keyword}%") ||
                EF.Functions.ILike(x.FullName, $"%{keyword}%"));
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            var value = department.Trim();

            query = query.Where(x =>EF.Functions.ILike(x.Department!, value));
        }

        if (!string.IsNullOrWhiteSpace(position))
        {
            var value = position.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.Position!, value));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        return await query.OrderBy(x => x.FullName).ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _context.Employees.AsNoTracking().FirstOrDefaultAsync(
                x => x.Id == id,cancellationToken);
    }

    public async Task<Employee?> GetEmployeeForUpdateAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> EmployeeNumberExistsAsync(string employeeNumber,
        Guid? excludeId = null,CancellationToken cancellationToken = default)
    {
        var query = _context.Employees.AsNoTracking().Where(x => x.EmployeeNumber == employeeNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> EmployeeEmailExistsAsync(string email,Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Employees
            .AsNoTracking()
            .Where(x => x.Email != null && x.Email == email);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddEmployeeAsync(Employee employee,CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(
            employee,
            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
