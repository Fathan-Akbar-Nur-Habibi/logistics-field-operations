using Logistics.Application.DTOs.Crew;

namespace Logistics.Application.Interfaces.Crew;

public interface ICrewService
{
    Task<List<EmployeeResponse>> GetEmployeesAsync(string? search = null, string? department = null,
        string? position = null,bool? isActive = null,CancellationToken cancellationToken = default);

    Task<EmployeeResponse?> GetEmployeeByIdAsync(Guid id,CancellationToken cancellationToken = default);

    Task<EmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request,
        CancellationToken cancellationToken = default);

    Task<EmployeeResponse> UpdateEmployeeAsync(Guid id,UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default);
}
