using Logistics.Domain.Entities;
namespace Logistics.Application.Interfaces.Maintenance;

public interface IMaintenanceRepository
{
    Task<List<MaintenancePlan>> GetMaintenancePlansAsync( string? search = null,
        Guid? assetId = null,bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<MaintenancePlan?> GetMaintenancePlanByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<MaintenancePlan?> GetMaintenancePlanForUpdateAsync(
        Guid id,CancellationToken cancellationToken = default);

    Task<bool> AssetExistsAsync(Guid assetId,
        CancellationToken cancellationToken = default);

    Task AddMaintenancePlanAsync(MaintenancePlan maintenancePlan,
        CancellationToken cancellationToken = default);

    Task<List<MaintenanceWorkOrder>> GetMaintenanceWorkOrdersAsync(
        string? search = null,
        Guid? assetId = null,
        string? status = null,
        string? priority = null,
        string? maintenanceType = null,
        CancellationToken cancellationToken = default);

    Task<MaintenanceWorkOrder?> GetMaintenanceWorkOrderByIdAsync(
        Guid id,CancellationToken cancellationToken = default);

    Task<MaintenanceWorkOrder?> GetMaintenanceWorkOrderForUpdateAsync(
        Guid id,CancellationToken cancellationToken = default);

    Task<bool> WorkOrderNumberExistsAsync(string workOrderNumber,
        CancellationToken cancellationToken = default);

    Task<MaintenancePlan?> GetMaintenancePlanForValidationAsync(
        Guid maintenancePlanId,CancellationToken cancellationToken = default);

    Task<bool> EmployeeExistsAsync(Guid employeeId,
        CancellationToken cancellationToken = default);

    Task AddMaintenanceWorkOrderAsync(MaintenanceWorkOrder workOrder,
        CancellationToken cancellationToken = default);

    Task<List<MaintenanceRecord>> GetMaintenanceRecordsAsync(Guid? maintenanceWorkOrderId = null,
        CancellationToken cancellationToken = default);

    Task<MaintenanceRecord?> GetMaintenanceRecordByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> MaintenanceWorkOrderExistsAsync(Guid workOrderId,
        CancellationToken cancellationToken = default);

    Task AddMaintenanceRecordAsync(MaintenanceRecord record,
        CancellationToken cancellationToken = default);

    Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
