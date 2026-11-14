using Logistics.Application.DTOs.Maintenance.Plan;
using Logistics.Application.DTOs.Maintenance.WorkOrder;
using Logistics.Application.DTOs.Maintenance.Record;
using Logistics.Application.Interfaces.Maintenance;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services.Maintenance;

public class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceRepository _repository;

    public MaintenanceService(IMaintenanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MaintenancePlanResponse>> GetMaintenancePlansAsync(
        string? search = null,
        Guid? assetId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var plans = await _repository.GetMaintenancePlansAsync(search, assetId, isActive, cancellationToken);
        return plans.Select(MapMaintenancePlan).ToList();
    }

    public async Task<MaintenancePlanResponse?> GetMaintenancePlanByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id, "Maintenance Plan ID");
        var plan = await _repository.GetMaintenancePlanByIdAsync(id, cancellationToken);
        return plan == null ? null : MapMaintenancePlan(plan);
    }

    public async Task<MaintenancePlanResponse> CreateMaintenancePlanAsync(CreateMaintenancePlanRequest request, CancellationToken cancellationToken = default)
    {
        ValidateId(request.AssetId, "Asset ID");
        ValidateRequiredText(request.PlanName, "PlanName");
        ValidateRequiredText(request.Description, "Description");

        if (request.IntervalDays <= 0 || request.IntervalDays > 3650)
        {
            throw new ArgumentException("IntervalDays must be between 1 and 3650.");
        }

        if (request.StartDate == default)
        {
            throw new ArgumentException("StartDate is invalid.");
        }

        var assetExists = await _repository.AssetExistsAsync(request.AssetId, cancellationToken);

        if (!assetExists)
        {
            throw new KeyNotFoundException("Asset was not found.");
        }

        var plan = new MaintenancePlan
        {
            AssetId = request.AssetId,
            PlanName = request.PlanName.Trim(),
            Description = request.Description.Trim(),
            IntervalDays = request.IntervalDays,
            StartDate = request.StartDate,
            NextMaintenanceDate = request.StartDate.AddDays(request.IntervalDays),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddMaintenancePlanAsync(plan, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        var createdPlan = await _repository.GetMaintenancePlanByIdAsync(plan.Id, cancellationToken);
        if (createdPlan == null)
        {
            throw new InvalidOperationException(
                "Failed to retrieve the created Maintenance Plan.");
        }

        return MapMaintenancePlan(createdPlan);
    }

    public async Task<MaintenancePlanResponse> UpdateMaintenancePlanAsync(
        Guid id, UpdateMaintenancePlanRequest request, CancellationToken cancellationToken = default)
    {
        ValidateId(id, "Maintenance Plan ID");

        ValidateRequiredText(request.PlanName, "PlanName");
        ValidateRequiredText(request.Description, "Description");

        if (request.IntervalDays <= 0 || request.IntervalDays > 3650)
        {
            throw new ArgumentException("IntervalDays must be between 1 and 3650.");
        }

        if (request.StartDate == default)
        {
            throw new ArgumentException("StartDate is invalid.");
        }

        var plan = await _repository.GetMaintenancePlanForUpdateAsync(id, cancellationToken);

        if (plan == null)
        {
            throw new KeyNotFoundException(
                "Maintenance Plan was not found.");
        }

        plan.PlanName = request.PlanName.Trim();
        plan.Description = request.Description.Trim();
        plan.IntervalDays = request.IntervalDays;
        plan.StartDate = request.StartDate;
        plan.NextMaintenanceDate = request.StartDate.AddDays(request.IntervalDays);
        plan.IsActive = request.IsActive;
        plan.UpdatedAt = DateTimeOffset.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        var updatedPlan = await _repository.GetMaintenancePlanByIdAsync(id, cancellationToken);
        if (updatedPlan == null)
        {
            throw new InvalidOperationException(
                "Failed to retrieve the updated Maintenance Plan.");
        }

        return MapMaintenancePlan(updatedPlan);
    }

    public async Task<List<MaintenanceWorkOrderResponse>>
        GetMaintenanceWorkOrdersAsync(
            string? search = null,
            Guid? assetId = null,
            string? status = null,
            string? priority = null,
            string? maintenanceType = null,
            CancellationToken cancellationToken = default)
    {
        var workOrders = await _repository.GetMaintenanceWorkOrdersAsync(search, assetId,
            NormalizeOptional(status),
            NormalizeOptional(priority),
            NormalizeOptional(maintenanceType), cancellationToken);
        return workOrders.Select(MapMaintenanceWorkOrder).ToList();
    }

    public async Task<MaintenanceWorkOrderResponse?>
        GetMaintenanceWorkOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ValidateId(id, "Work Order ID");

        var workOrder = await _repository.GetMaintenanceWorkOrderByIdAsync(
            id,
            cancellationToken);

        return workOrder == null
            ? null
            : MapMaintenanceWorkOrder(workOrder);
    }

    public async Task<MaintenanceWorkOrderResponse>
        CreateMaintenanceWorkOrderAsync(CreateMaintenanceWorkOrderRequest request,
            CancellationToken cancellationToken = default)
    {
        ValidateId(request.AssetId, "Asset ID");
        ValidateRequiredText(request.Title, "Title");
        ValidateRequiredText(request.Description, "Description");

        if (request.ScheduledDate == default)
        {
            throw new ArgumentException("ScheduledDate is invalid.");
        }

        var maintenanceType = NormalizeRequired(request.MaintenanceType, "MaintenanceType");
        if (maintenanceType != "PREVENTIVE" &&
            maintenanceType != "CORRECTIVE")
        {
            throw new ArgumentException("MaintenanceType must be PREVENTIVE or CORRECTIVE.");
        }
        var priority = NormalizeRequired(request.Priority, "Priority");
        ValidatePriority(priority);
        ValidateNonNegative(request.EstimatedCost, "EstimatedCost");

        var assetExists = await _repository.AssetExistsAsync(request.AssetId, cancellationToken);

        if (!assetExists)
        {
            throw new KeyNotFoundException("Asset was not found.");
        }

        if (request.MaintenancePlanId.HasValue)
        {
            ValidateId(
                request.MaintenancePlanId.Value,
                "Maintenance Plan ID");

            var plan = await _repository.GetMaintenancePlanForValidationAsync(
                request.MaintenancePlanId.Value,
                cancellationToken);

            if (plan == null)
            {
                throw new KeyNotFoundException("Maintenance Plan was not found.");
            }

            if (!plan.IsActive)
            {
                throw new InvalidOperationException(
                    "Maintenance Plan is inactive.");
            }

            if (plan.AssetId != request.AssetId)
            {
                throw new ArgumentException("Maintenance Plan does not belong to the selected Asset.");
            }
        }

        if (request.AssignedToEmployeeId.HasValue)
        {
            ValidateId(request.AssignedToEmployeeId.Value, "Employee ID");
            var employeeExists = await _repository.EmployeeExistsAsync(request.AssignedToEmployeeId.Value, cancellationToken);

            if (!employeeExists)
            {
                throw new KeyNotFoundException("Assigned Employee was not found.");
            }
        }

        var workOrder = new MaintenanceWorkOrder
        {
            WorkOrderNumber = GenerateWorkOrderNumber(),
            AssetId = request.AssetId,
            MaintenancePlanId = request.MaintenancePlanId,
            MaintenanceType = maintenanceType,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ScheduledDate = request.ScheduledDate,
            Priority = priority,
            Status = "OPEN",
            EstimatedCost = request.EstimatedCost,
            ActualCost = null,
            AssignedToEmployeeId = request.AssignedToEmployeeId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddMaintenanceWorkOrderAsync(workOrder, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        var createdWorkOrder = await _repository.GetMaintenanceWorkOrderByIdAsync(workOrder.Id, cancellationToken);

        if (createdWorkOrder == null)
        {
            throw new InvalidOperationException("Failed to retrieve the created Work Order.");
        }

        return MapMaintenanceWorkOrder(createdWorkOrder);
    }

    public async Task<MaintenanceWorkOrderResponse>
        UpdateMaintenanceWorkOrderAsync(Guid id, UpdateMaintenanceWorkOrderRequest request, CancellationToken cancellationToken = default)
    {
        ValidateId(id, "Work Order ID");

        ValidateRequiredText(request.Title, "Title");
        ValidateRequiredText(request.Description, "Description");

        if (request.ScheduledDate == default)
        {
            throw new ArgumentException("ScheduledDate is invalid.");
        }

        var maintenanceType = NormalizeRequired(request.MaintenanceType, "MaintenanceType");

        if (maintenanceType != "PREVENTIVE" &&
            maintenanceType != "CORRECTIVE")
        {
            throw new ArgumentException("MaintenanceType must be PREVENTIVE or CORRECTIVE.");
        }

        var priority = NormalizeRequired(request.Priority, "Priority");
        ValidatePriority(priority);
        ValidateNonNegative(request.EstimatedCost, "EstimatedCost");

        var workOrder = await _repository.GetMaintenanceWorkOrderForUpdateAsync(id, cancellationToken);

        if (workOrder == null)
        {
            throw new KeyNotFoundException("Maintenance Work Order was not found.");
        }

        if (workOrder.Status == "COMPLETED" ||
            workOrder.Status == "CANCELLED")
        {
            throw new InvalidOperationException("Completed or cancelled Work Orders cannot be updated.");
        }

        if (request.AssignedToEmployeeId.HasValue)
        {
            ValidateId(request.AssignedToEmployeeId.Value, "Employee ID");

            var employeeExists = await _repository.EmployeeExistsAsync(request.AssignedToEmployeeId.Value, cancellationToken);
            if (!employeeExists)
            {
                throw new KeyNotFoundException("Assigned Employee was not found.");
            }
        }

        workOrder.MaintenanceType = maintenanceType;
        workOrder.Title = request.Title.Trim();
        workOrder.Description = request.Description.Trim();
        workOrder.ScheduledDate = request.ScheduledDate;
        workOrder.Priority = priority;
        workOrder.EstimatedCost = request.EstimatedCost;
        workOrder.AssignedToEmployeeId = request.AssignedToEmployeeId;
        workOrder.UpdatedAt = DateTimeOffset.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        var updatedWorkOrder =
            await _repository.GetMaintenanceWorkOrderByIdAsync(id, cancellationToken);

        if (updatedWorkOrder == null)
        {
            throw new InvalidOperationException(
                "Failed to retrieve the updated Work Order.");
        }
        return MapMaintenanceWorkOrder(updatedWorkOrder);
    }

    public async Task<MaintenanceWorkOrderResponse> UpdateMaintenanceWorkOrderStatusAsync(
        Guid id, UpdateMaintenanceWorkOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        ValidateId(id, "Work Order ID");

        var newStatus = NormalizeRequired(request.Status, "Status");

        if (newStatus != "IN_PROGRESS" &&
            newStatus != "COMPLETED" &&
            newStatus != "CANCELLED")
        {
            throw new ArgumentException(
                "Status must be IN_PROGRESS, COMPLETED, or CANCELLED.");
        }

        var workOrder = await _repository.GetMaintenanceWorkOrderForUpdateAsync(
            id,
            cancellationToken);

        if (workOrder == null)
        {
            throw new KeyNotFoundException(
                "Maintenance Work Order was not found.");
        }

        var currentStatus = workOrder.Status.ToUpperInvariant();

        // Status terminal tidak boleh diubah lagi.
        if (currentStatus == "COMPLETED" ||
            currentStatus == "CANCELLED")
        {
            throw new InvalidOperationException(
                "Completed or cancelled Work Orders cannot change status.");
        }

        // Validasi transisi status.
        if (currentStatus == "OPEN")
        {
            if (newStatus != "IN_PROGRESS" &&
                newStatus != "CANCELLED")
            {
                throw new InvalidOperationException(
                    "An OPEN Work Order can only transition to IN_PROGRESS or CANCELLED.");
            }
        }
        else if (currentStatus == "IN_PROGRESS")
        {
            if (newStatus != "COMPLETED" &&
                newStatus != "CANCELLED")
            {
                throw new InvalidOperationException(
                    "An IN_PROGRESS Work Order can only transition to COMPLETED or CANCELLED.");
            }
        }
        else
        {
            throw new InvalidOperationException(
                $"Unsupported current Work Order status: {currentStatus}.");
        }

        if (newStatus == "COMPLETED" &&
            !workOrder.Records.Any())
        {
            throw new InvalidOperationException(
                "A Work Order must have at least one Maintenance Record before completion.");
        }

        workOrder.Status = newStatus;

        if (newStatus == "COMPLETED")
        {
            workOrder.CompletedDate = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        workOrder.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedWorkOrder =
            await _repository.GetMaintenanceWorkOrderByIdAsync(
                id,
                cancellationToken);

        if (updatedWorkOrder == null)
        {
            throw new InvalidOperationException(
                "Failed to retrieve the updated Work Order.");
        }

        return MapMaintenanceWorkOrder(updatedWorkOrder);
    }

    public async Task<List<MaintenanceRecordResponse>>
        GetMaintenanceRecordsAsync(Guid? maintenanceWorkOrderId = null, CancellationToken cancellationToken = default)
    {
        if (maintenanceWorkOrderId.HasValue)
        {
            ValidateId(maintenanceWorkOrderId.Value, "Work Order ID");
        }

        var records = await _repository.GetMaintenanceRecordsAsync(maintenanceWorkOrderId, cancellationToken);
        return records.Select(MapMaintenanceRecord).ToList();
    }

    public async Task<MaintenanceRecordResponse?>
        GetMaintenanceRecordByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ValidateId(id, "Maintenance Record ID");

        var record = await _repository.GetMaintenanceRecordByIdAsync(id, cancellationToken);
        return record == null ? null : MapMaintenanceRecord(record);
    }

    public async Task<MaintenanceRecordResponse>
        CreateMaintenanceRecordAsync(CreateMaintenanceRecordRequest request, CancellationToken cancellationToken = default)
    {
        ValidateId(request.MaintenanceWorkOrderId, "Work Order ID");

        if (request.MaintenanceDate == default)
        {
            throw new ArgumentException("MaintenanceDate is invalid.");
        }

        ValidateRequiredText(request.WorkDescription, "WorkDescription");
        ValidateNonNegative(request.LaborCost, "LaborCost");
        ValidateNonNegative(request.PartsCost, "PartsCost");

        if (request.PerformedByEmployeeId.HasValue)
        {
            ValidateId(
                request.PerformedByEmployeeId.Value,
                "Employee ID");
        }

        var recordId = await _repository.ExecuteInTransactionAsync(
            async ct =>
            {
                var workOrder = await _repository.GetMaintenanceWorkOrderForUpdateAsync(request.MaintenanceWorkOrderId, ct);

                if (workOrder == null)
                {
                    throw new KeyNotFoundException(
                        "Maintenance Work Order was not found.");
                }

                if (workOrder.Status != "OPEN" &&
                    workOrder.Status != "IN_PROGRESS")
                {
                    throw new InvalidOperationException(
                        "Maintenance Record can only be added to an OPEN or IN_PROGRESS Work Order.");
                }

                if (request.PerformedByEmployeeId.HasValue)
                {
                    var employeeExists = await _repository.EmployeeExistsAsync(request.PerformedByEmployeeId.Value, ct);
                    if (!employeeExists)
                    {
                        throw new KeyNotFoundException("Performed Employee was not found.");
                    }
                }

                var record = new MaintenanceRecord
                {
                    MaintenanceWorkOrderId = request.MaintenanceWorkOrderId,
                    PerformedByEmployeeId = request.PerformedByEmployeeId,
                    MaintenanceDate = request.MaintenanceDate.ToUniversalTime(),
                    WorkDescription = request.WorkDescription.Trim(),
                    PartsReplaced = NormalizeOptional(request.PartsReplaced),
                    LaborCost = request.LaborCost,
                    PartsCost = request.PartsCost,
                    Notes = NormalizeOptional(request.Notes),
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await _repository.AddMaintenanceRecordAsync(record, ct);

                // Calculate the total cost of all Maintenance Records
                var laborTotal = workOrder.Records
                    .Where(x => x.Id != record.Id)
                    .Sum(x => x.LaborCost ?? 0m);

                var partsTotal = workOrder.Records
                    .Where(x => x.Id != record.Id)
                    .Sum(x => x.PartsCost ?? 0m);

                // Add a one-time fee to the record being created
                workOrder.ActualCost =
                    laborTotal +
                    partsTotal +
                    (record.LaborCost ?? 0m) +
                    (record.PartsCost ?? 0m);

                workOrder.Status = "IN_PROGRESS";
                workOrder.UpdatedAt = DateTimeOffset.UtcNow;

                await _repository.SaveChangesAsync(ct);

                return record.Id;
            }, cancellationToken);

        var createdRecord = await _repository.GetMaintenanceRecordByIdAsync(recordId, cancellationToken);

        if (createdRecord == null)
        {
            throw new InvalidOperationException("Failed to retrieve the created Maintenance Record.");
        }

        return MapMaintenanceRecord(createdRecord);
    }

    private static MaintenancePlanResponse MapMaintenancePlan(MaintenancePlan plan)
    {
        return new MaintenancePlanResponse
        {
            Id = plan.Id,
            AssetId = plan.AssetId,
            AssetCode = plan.Asset.AssetCode,
            AssetName = plan.Asset.Name,
            PlanName = plan.PlanName,
            Description = plan.Description,
            IntervalDays = plan.IntervalDays,
            StartDate = plan.StartDate,
            NextMaintenanceDate = plan.NextMaintenanceDate,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }

    private static MaintenanceWorkOrderResponse MapMaintenanceWorkOrder(MaintenanceWorkOrder workOrder)
    {
        return new MaintenanceWorkOrderResponse
        {
            Id = workOrder.Id,
            WorkOrderNumber = workOrder.WorkOrderNumber,
            AssetId = workOrder.AssetId,
            AssetCode = workOrder.Asset.AssetCode,
            AssetName = workOrder.Asset.Name,
            MaintenancePlanId = workOrder.MaintenancePlanId,
            MaintenancePlanName = workOrder.MaintenancePlan?.PlanName,
            MaintenanceType = workOrder.MaintenanceType,
            Title = workOrder.Title,
            Description = workOrder.Description,
            ScheduledDate = workOrder.ScheduledDate,
            CompletedDate = workOrder.CompletedDate,
            Priority = workOrder.Priority,
            Status = workOrder.Status,
            EstimatedCost = workOrder.EstimatedCost,
            ActualCost = workOrder.ActualCost,
            AssignedToEmployeeId = workOrder.AssignedToEmployeeId,
            AssignedToEmployeeName = workOrder.AssignedToEmployee?.FullName,
            CreatedByUserId = workOrder.CreatedByUserId,
            CreatedAt = workOrder.CreatedAt,
            UpdatedAt = workOrder.UpdatedAt
        };
    }

    private static MaintenanceRecordResponse MapMaintenanceRecord(MaintenanceRecord record)
    {
        return new MaintenanceRecordResponse
        {
            Id = record.Id,
            MaintenanceWorkOrderId = record.MaintenanceWorkOrderId,
            WorkOrderNumber = record.MaintenanceWorkOrder.WorkOrderNumber,
            WorkOrderTitle = record.MaintenanceWorkOrder.Title,
            AssetId = record.MaintenanceWorkOrder.AssetId,
            AssetCode = record.MaintenanceWorkOrder.Asset.AssetCode,
            AssetName = record.MaintenanceWorkOrder.Asset.Name,
            PerformedByEmployeeId = record.PerformedByEmployeeId,
            PerformedByEmployeeName = record.PerformedByEmployee?.FullName,
            MaintenanceDate = record.MaintenanceDate,
            WorkDescription = record.WorkDescription,
            PartsReplaced = record.PartsReplaced,
            LaborCost = record.LaborCost,
            PartsCost = record.PartsCost,
            ActualCost = record.MaintenanceWorkOrder.ActualCost ?? 0m,
            Notes = record.Notes,
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };
    }

    private static void ValidateId(Guid id, string fieldName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException($"{fieldName} cannot be empty.");
        }
    }

    private static void ValidateRequiredText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{fieldName} is required.");
        }
    }

    private static void ValidateNonNegative(decimal? value, string fieldName)
    {
        if (value.HasValue && value.Value < 0)
        {
            throw new ArgumentException(
                $"{fieldName} cannot be negative.");
        }
    }

    private static string NormalizeRequired(string? value, string fieldName)
    {
        ValidateRequiredText(value, fieldName);
        return value!.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static void ValidatePriority(string priority)
    {
        var allowedPriorities = new[]
        {
            "LOW",
            "MEDIUM",
            "HIGH",
            "CRITICAL"
        };

        if (!allowedPriorities.Contains(priority))
        {
            throw new ArgumentException(
                "Priority must be LOW, MEDIUM, HIGH, or CRITICAL.");
        }
    }

    private static string GenerateWorkOrderNumber()
    {
        return $"WO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"
            .Substring(0, 20)
            .ToUpperInvariant();
    }
}
