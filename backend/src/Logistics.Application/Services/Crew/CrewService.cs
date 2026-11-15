
using Logistics.Application.DTOs.Crew;
using Logistics.Application.Interfaces.Crew;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services.Crew;

public class CrewService : ICrewService
{
    private readonly ICrewRepository _repository;

    public CrewService(ICrewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EmployeeResponse>> GetEmployeesAsync(
        string? search = null,
        string? department = null,
        string? position = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var employees = await _repository.GetEmployeesAsync(
            search,
            NormalizeOptional(department),
            NormalizeOptional(position),
            isActive,
            cancellationToken);

        return employees.Select(MapEmployeeToResponse).ToList();
    }

    public async Task<EmployeeResponse?> GetEmployeeByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        var employee = await _repository.GetEmployeeByIdAsync(id,cancellationToken);

        return employee == null ? null : MapEmployeeToResponse(employee);
    }

    public async Task<EmployeeResponse> CreateEmployeeAsync(CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.EmployeeNumber))
        {
            throw new ArgumentException("Employee number is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new ArgumentException("Employee full name is required.");
        }

        var employeeNumber = NormalizeEmployeeNumber(request.EmployeeNumber);

        var fullName = request.FullName.Trim();

        var email = NormalizeEmail(request.Email);

        if (await _repository.EmployeeNumberExistsAsync(employeeNumber,cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"Employee number '{employeeNumber}' already exists.");
        }

        if (email != null && await _repository.EmployeeEmailExistsAsync(
                email,cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"Email '{email}' is already registered.");
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),

            EmployeeNumber = employeeNumber,
            FullName = fullName,
            Email = email,

            PhoneNumber = NormalizeOptional(request.PhoneNumber),
            Position = NormalizeOptional(request.Position),
            Department = NormalizeOptional(request.Department),

            HireDate = request.HireDate,

            IsActive = true,

            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddEmployeeAsync(employee,cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        var createdEmployee = await _repository.GetEmployeeByIdAsync(employee.Id,cancellationToken);

        if (createdEmployee == null)
        {
            throw new InvalidOperationException("Failed to retrieve the created employee.");
        }

        return MapEmployeeToResponse(createdEmployee);
    }

    public async Task<EmployeeResponse> UpdateEmployeeAsync(Guid id,UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        var employee = await _repository.GetEmployeeForUpdateAsync(id,cancellationToken);

        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID '{id}' was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new ArgumentException("Employee full name is required.");
        }

        var email = NormalizeEmail(request.Email);

        if (email != null &&
            await _repository.EmployeeEmailExistsAsync(email,id,cancellationToken))
        {
            throw new InvalidOperationException(
                $"Email '{email}' already belongs to another employee.");
        }

        employee.FullName = request.FullName.Trim();

        employee.Email = email;

        employee.PhoneNumber = NormalizeOptional(request.PhoneNumber);

        employee.Position = NormalizeOptional(request.Position);

        employee.Department = NormalizeOptional(request.Department);

        employee.HireDate = request.HireDate;

        employee.IsActive = request.IsActive;

        employee.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedEmployee = await _repository.GetEmployeeByIdAsync(
            id,cancellationToken);

        if (updatedEmployee == null)
        {
            throw new InvalidOperationException("Failed to retrieve the updated employee.");
        }

        return MapEmployeeToResponse(updatedEmployee);
    }

    private static EmployeeResponse MapEmployeeToResponse(Employee employee)
    {
        return new EmployeeResponse
        {
            Id = employee.Id,

            EmployeeNumber = employee.EmployeeNumber,
            FullName = employee.FullName,
            Email = employee.Email,

            PhoneNumber = employee.PhoneNumber,
            Position = employee.Position,
            Department = employee.Department,

            HireDate = employee.HireDate,

            IsActive = employee.IsActive,

            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }

    private static void ValidateId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Employee ID is required.");
        }
    }

    private static string NormalizeEmployeeNumber(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeEmail(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null
            : value.Trim().ToLowerInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null
            : value.Trim();
    }
}
