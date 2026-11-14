using Logistics.Application.DTOs.Maintenance.Plan;
using Logistics.Application.DTOs.Maintenance.WorkOrder;
using Logistics.Application.DTOs.Maintenance.Record;
namespace Logistics.Application.Interfaces.Maintenance;

public interface IMaintenanceService
{
    Task<List<MaintenancePlanResponse>> GetMaintenancePlansAsync(
        string? search = null,
        Guid? assetId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<MaintenancePlanResponse?> GetMaintenancePlanByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<MaintenancePlanResponse> CreateMaintenancePlanAsync(
        CreateMaintenancePlanRequest request,CancellationToken cancellationToken = default);

    Task<MaintenancePlanResponse> UpdateMaintenancePlanAsync(
        Guid id,UpdateMaintenancePlanRequest request,CancellationToken cancellationToken = default);

    Task<List<MaintenanceWorkOrderResponse>> GetMaintenanceWorkOrdersAsync(
        string? search = null,
        Guid? assetId = null,
        string? status = null,
        string? priority = null,
        string? maintenanceType = null,
        CancellationToken cancellationToken = default);

    Task<MaintenanceWorkOrderResponse?> GetMaintenanceWorkOrderByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<MaintenanceWorkOrderResponse> CreateMaintenanceWorkOrderAsync(CreateMaintenanceWorkOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<MaintenanceWorkOrderResponse> UpdateMaintenanceWorkOrderAsync(Guid id,
        UpdateMaintenanceWorkOrderRequest request,CancellationToken cancellationToken = default);

    Task<MaintenanceWorkOrderResponse> UpdateMaintenanceWorkOrderStatusAsync(
    Guid id,UpdateMaintenanceWorkOrderStatusRequest request,CancellationToken cancellationToken = default);
    Task<List<MaintenanceRecordResponse>> GetMaintenanceRecordsAsync(Guid? maintenanceWorkOrderId = null,
        CancellationToken cancellationToken = default);

    Task<MaintenanceRecordResponse?> GetMaintenanceRecordByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<MaintenanceRecordResponse> CreateMaintenanceRecordAsync(CreateMaintenanceRecordRequest request,
        CancellationToken cancellationToken = default);
}
