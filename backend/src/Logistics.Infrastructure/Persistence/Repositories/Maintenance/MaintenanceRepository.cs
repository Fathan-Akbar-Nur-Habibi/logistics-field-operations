using Logistics.Application.Interfaces.Maintenance;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Repositories.Maintenance;

public class MaintenanceRepository : IMaintenanceRepository
{
    private readonly LogisticsDbContext _context;

    public MaintenanceRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<List<MaintenancePlan>> GetMaintenancePlansAsync(
        string? search = null,
        Guid? assetId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.MaintenancePlans
            .AsNoTracking()
            .Include(x => x.Asset)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.PlanName, $"%{keyword}%") ||
                EF.Functions.ILike(x.Description, $"%{keyword}%") ||
                EF.Functions.ILike(x.Asset.AssetCode, $"%{keyword}%") ||
                EF.Functions.ILike(x.Asset.Name, $"%{keyword}%"));
        }

        if (assetId.HasValue)
        {
            query = query.Where(x => x.AssetId == assetId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(x => x.NextMaintenanceDate)
            .ThenBy(x => x.PlanName)
            .ToListAsync(cancellationToken);
    }

    public async Task<MaintenancePlan?> GetMaintenancePlanByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.MaintenancePlans
            .AsNoTracking()
            .Include(x => x.Asset)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<MaintenancePlan?> GetMaintenancePlanForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.MaintenancePlans
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> AssetExistsAsync(
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AnyAsync(
                x => x.Id == assetId,
                cancellationToken);
    }

    public async Task AddMaintenancePlanAsync(MaintenancePlan maintenancePlan,
        CancellationToken cancellationToken = default)
    {
        await _context.MaintenancePlans.AddAsync(
            maintenancePlan,
            cancellationToken);
    }

    public async Task<List<MaintenanceWorkOrder>> GetMaintenanceWorkOrdersAsync(
        string? search = null,
        Guid? assetId = null,
        string? status = null,
        string? priority = null,
        string? maintenanceType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.MaintenanceWorkOrders
            .AsNoTracking()
            .Include(x => x.Asset)
            .Include(x => x.MaintenancePlan)
            .Include(x => x.AssignedToEmployee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.WorkOrderNumber, $"%{keyword}%") ||
                EF.Functions.ILike(x.Title, $"%{keyword}%") ||
                EF.Functions.ILike(x.Description, $"%{keyword}%") ||
                EF.Functions.ILike(x.Asset.AssetCode, $"%{keyword}%") ||
                EF.Functions.ILike(x.Asset.Name, $"%{keyword}%"));
        }

        if (assetId.HasValue)
        {
            query = query.Where(x => x.AssetId == assetId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x =>
                x.Status == status.Trim().ToUpper());
        }

        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = query.Where(x =>
                x.Priority == priority.Trim().ToUpper());
        }

        if (!string.IsNullOrWhiteSpace(maintenanceType))
        {
            query = query.Where(x =>
                x.MaintenanceType == maintenanceType.Trim().ToUpper());
        }

        return await query
            .OrderByDescending(x => x.ScheduledDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MaintenanceWorkOrder?> GetMaintenanceWorkOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceWorkOrders
            .AsNoTracking()
            .Include(x => x.Asset)
            .Include(x => x.MaintenancePlan)
            .Include(x => x.AssignedToEmployee)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<MaintenanceWorkOrder?> GetMaintenanceWorkOrderForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceWorkOrders
            .Include(x => x.Records)
            .FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
    }

    public async Task<bool> WorkOrderNumberExistsAsync(string workOrderNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceWorkOrders
            .AnyAsync(x => x.WorkOrderNumber == workOrderNumber,cancellationToken);
    }

    public async Task<MaintenancePlan?> GetMaintenancePlanForValidationAsync(
        Guid maintenancePlanId,CancellationToken cancellationToken = default)
    {
        return await _context.MaintenancePlans
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == maintenancePlanId,
                cancellationToken);
    }

    public async Task<bool> EmployeeExistsAsync(Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(x => x.Id == employeeId,cancellationToken);
    }

    public async Task AddMaintenanceWorkOrderAsync(MaintenanceWorkOrder workOrder,
        CancellationToken cancellationToken = default)
    {
        await _context.MaintenanceWorkOrders.AddAsync(workOrder,
            cancellationToken);
    }

    public async Task<List<MaintenanceRecord>> GetMaintenanceRecordsAsync(
        Guid? maintenanceWorkOrderId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.MaintenanceRecords
            .AsNoTracking()
            .Include(x => x.MaintenanceWorkOrder)
                .ThenInclude(x => x.Asset)
            .Include(x => x.PerformedByEmployee)
            .AsQueryable();

        if (maintenanceWorkOrderId.HasValue)
        {
            query = query.Where(x =>x.MaintenanceWorkOrderId ==
                maintenanceWorkOrderId.Value);
        }

        return await query
            .OrderByDescending(x => x.MaintenanceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<MaintenanceRecord?> GetMaintenanceRecordByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceRecords
            .AsNoTracking()
            .Include(x => x.MaintenanceWorkOrder)
                .ThenInclude(x => x.Asset)
            .Include(x => x.PerformedByEmployee)
            .FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
    }

    public async Task<bool> MaintenanceWorkOrderExistsAsync(Guid workOrderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceWorkOrders
            .AnyAsync(x => x.Id == workOrderId,cancellationToken);
    }

    public async Task AddMaintenanceRecordAsync(MaintenanceRecord record,
        CancellationToken cancellationToken = default)
    {
        await _context.MaintenanceRecords.AddAsync(record,
            cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var result = await operation(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
{
    try
    {
        await transaction.RollbackAsync(CancellationToken.None);
    }
    catch (Exception)
    {
        // Retain the original exception if the rollback fails
    }

    throw;
}
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
