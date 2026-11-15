using Logistics.Domain.Entities;

namespace Logistics.Application.Interfaces.Crew;

public interface ICrewRepository
{
    Task<List<Employee>> GetEmployeesAsync(string? search = null, string? department = null,
        string? position = null, bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeForUpdateAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> EmployeeNumberExistsAsync(string employeeNumber,Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmployeeEmailExistsAsync(string email,Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddEmployeeAsync(Employee employee,CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
