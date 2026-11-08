
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Seed;

public static class OperationalHistorySeeder
{
    public static async Task SeedAsync(
        LogisticsDbContext context,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // 1. LOAD MASTER DATA

        var items = await context.Items
            .ToDictionaryAsync(x => x.Code, cancellationToken);

        var locations = await context.Locations
            .ToDictionaryAsync(x => x.Code, cancellationToken);

        var assets = await context.Assets
            .ToDictionaryAsync(x => x.AssetCode, cancellationToken);

        if (items.Count == 0 || locations.Count == 0 || assets.Count == 0)
        {
            throw new InvalidOperationException(
                "Operational master data is not yet available. " +
                "Please ensure OperationalDataSeeder has been executed.");
        }

        // 2. STOCK TRANSACTIONS - 20 DATA

        var stockTransactionData = new[]
        {
            ("ITM-ELEC-001", "WH-JKT", "IN", 10m, -30),
            ("ITM-ELEC-001", "WH-JKT", "OUT", 2m, -25),

            ("ITM-ELEC-002", "WH-JKT", "IN", 15m, -28),
            ("ITM-ELEC-002", "WH-JKT", "OUT", 3m, -20),

            ("ITM-OFF-001", "WH-JKT", "IN", 30m, -25),
            ("ITM-OFF-001", "WH-JKT", "OUT", 5m, -18),

            ("ITM-OFF-002", "WH-JKT", "IN", 20m, -22),
            ("ITM-OFF-002", "WH-JKT", "OUT", 2m, -15),

            ("ITM-SAFE-001", "DC-BKS", "IN", 50m, -20),
            ("ITM-SAFE-001", "DC-BKS", "OUT", 10m, -14),

            ("ITM-SAFE-002", "DC-BKS", "IN", 40m, -18),
            ("ITM-SAFE-002", "DC-BKS", "OUT", 5m, -12),

            ("ITM-TOOLS-001", "DC-BKS", "IN", 5m, -16),
            ("ITM-TOOLS-001", "DC-BKS", "OUT", 1m, -10),

            ("ITM-TOOLS-002", "HUB-BDG", "IN", 10m, -14),
            ("ITM-TOOLS-002", "HUB-BDG", "OUT", 2m, -8),

            ("ITM-SP-001", "HUB-SBY", "IN", 15m, -12),
            ("ITM-SP-001", "HUB-SBY", "OUT", 4m, -6),

            ("ITM-SP-002", "HUB-SBY", "IN", 25m, -10),
            ("ITM-SP-002", "HUB-SBY", "OUT", 5m, -3)
        };

        var existingTransactionNumbers = await context.StockTransactions
            .Select(x => x.TransactionNumber)
            .ToHashSetAsync(cancellationToken);

        var stockTransactions = new List<StockTransaction>();

        for (var i = 0; i < stockTransactionData.Length; i++)
        {
            var data = stockTransactionData[i];

            var transactionNumber = $"TRX-DEMO-{i + 1:D3}";

            if (existingTransactionNumbers.Contains(transactionNumber))
                continue;

            var item = GetRequired(items, data.Item1, "Item");
            var location = GetRequired(locations, data.Item2, "Location");

            stockTransactions.Add(new StockTransaction
            {
                Id = CreateId("A1000000", i + 1),
                TransactionNumber = transactionNumber,
                ItemId = item.Id,
                LocationId = location.Id,
                TransactionType = data.Item3,
                Quantity = data.Item4,
                UnitPrice = item.UnitPrice ?? 0m,
                ReferenceNumber = $"REF-DEMO-{i + 1:D3}",
                Notes = $"Demo stock transaction {data.Item3}",
                TransactionDate = DateTimeOffset.UtcNow.AddDays(data.Item5)
            });
        }

        if (stockTransactions.Count > 0)
        {
            context.StockTransactions.AddRange(stockTransactions);
            await context.SaveChangesAsync(cancellationToken);
        }

        // 3. ASSET MOVEMENTS - 10 DATA
        var existingMovementIds = await context.AssetMovements
    .Select(x => x.Id)
    .ToHashSetAsync(cancellationToken);

        var locationList = locations.Values
            .OrderBy(x => x.Code)
            .ToList();

        var assetMovements = new List<AssetMovement>();

        var assetCodes = assets.Keys
            .OrderBy(x => x)
            .ToList();

        for (var i = 0; i < assetCodes.Count; i++)
        {
            var asset = assets[assetCodes[i]];

            var destinationLocation = locationList.FirstOrDefault(
                x => x.Id == asset.LocationId);

            if (destinationLocation == null)
            {
                throw new InvalidOperationException(
                    $"Lokasi aset {asset.AssetCode} tidak ditemukan.");
            }

            var fromLocation = locationList.FirstOrDefault(
                x => x.Id != destinationLocation.Id);

            if (fromLocation == null)
            {
                throw new InvalidOperationException(
                    "Minimal dibutuhkan dua lokasi untuk asset movement.");
            }

            // Gunakan ID tetap sebagai identitas seed.
            var movementId = CreateId("A2000000", i + 1);

            // Cek berdasarkan primary key, bukan tanggal.
            if (existingMovementIds.Contains(movementId))
                continue;

            var movementDate = DateTimeOffset.UtcNow.AddDays(-30 + i);

            assetMovements.Add(new AssetMovement
            {
                Id = movementId,
                AssetId = asset.Id,
                FromLocationId = fromLocation.Id,
                ToLocationId = destinationLocation.Id,
                MovementDate = movementDate,
                Reason = $"Demo asset relocation to {destinationLocation.Name}",
                MovedByUserId = null
            });
        }

        if (assetMovements.Count > 0)
        {
            context.AssetMovements.AddRange(assetMovements);
            await context.SaveChangesAsync(cancellationToken);
        }

        // 4. MAINTENANCE PLANS - 10 DATA
        var existingPlans = await context.MaintenancePlans
            .ToListAsync(cancellationToken);

        var maintenancePlans = new List<MaintenancePlan>();

        var planByAssetId = existingPlans
            .GroupBy(x => x.AssetId)
            .ToDictionary(x => x.Key, x => x.First());

        var nextMaintenanceOffsets = new[]
        {
            -30, -15, -5, 3, 7,
            14, 30, 45, 60, 90
        };

        for (var i = 0; i < assetCodes.Count; i++)
        {
            var asset = assets[assetCodes[i]];

            if (planByAssetId.ContainsKey(asset.Id))
                continue;

            var plan = new MaintenancePlan
            {
                Id = CreateId("A3000000", i + 1),
                AssetId = asset.Id,
                PlanName = $"DEMO-MP-{i + 1:D3}",
                Description = $"Scheduled preventive maintenance for {asset.Name}",
                IntervalDays = i % 3 == 0 ? 30 : i % 3 == 1 ? 60 : 90,
                StartDate = today.AddDays(-90),
                NextMaintenanceDate = today.AddDays(nextMaintenanceOffsets[i]),
                IsActive = true
            };

            maintenancePlans.Add(plan);
            planByAssetId[asset.Id] = plan;
        }

        if (maintenancePlans.Count > 0)
        {
            context.MaintenancePlans.AddRange(maintenancePlans);
            await context.SaveChangesAsync(cancellationToken);
        }

        // 5. MAINTENANCE WORK ORDERS - 10 DATA

        var existingWorkOrderNumbers = await context.MaintenanceWorkOrders
            .Select(x => x.WorkOrderNumber)
            .ToHashSetAsync(cancellationToken);

        var workOrders = new List<MaintenanceWorkOrder>();

        var scheduledOffsets = new[]
        {
            -40, -30, -20, -15, -10,
            -15, -7, -3, 5, 12
        };

        for (var i = 0; i < assetCodes.Count; i++)
        {
            var asset = assets[assetCodes[i]];

            var workOrderNumber = $"WO-DEMO-{i + 1:D3}";

            if (existingWorkOrderNumbers.Contains(workOrderNumber))
                continue;

            planByAssetId.TryGetValue(asset.Id, out var plan);

            var scheduledDate = today.AddDays(scheduledOffsets[i]);

            var isCompleted = i < 5;
            var isInProgress = i >= 8;

            workOrders.Add(new MaintenanceWorkOrder
            {
                Id = CreateId("A4000000", i + 1),
                WorkOrderNumber = workOrderNumber,
                AssetId = asset.Id,
                MaintenancePlanId = plan?.Id,
                MaintenanceType = i % 2 == 0
                    ? "PREVENTIVE"
                    : "CORRECTIVE",
                Title = $"Maintenance {asset.Name}",
                Description = $"Demo maintenance work order for {asset.AssetCode}",
                ScheduledDate = scheduledDate,
                CompletedDate = isCompleted
                    ? scheduledDate.AddDays(2)
                    : null,
                Priority = i % 3 == 0 ? "HIGH" : "MEDIUM",
                Status = isCompleted
                    ? "COMPLETED"
                    : isInProgress
                        ? "IN_PROGRESS"
                        : "OPEN",
                EstimatedCost = 500000m + (i * 250000m),
                ActualCost = isCompleted
                    ? 450000m + (i * 200000m)
                    : null,
                AssignedToEmployeeId = null,
                CreatedByUserId = null
            });
        }

        if (workOrders.Count > 0)
        {
            context.MaintenanceWorkOrders.AddRange(workOrders);
            await context.SaveChangesAsync(cancellationToken);
        }

        // 6. MAINTENANCE RECORDS - 5 DATA
        var completedWorkOrders = await context.MaintenanceWorkOrders
            .Where(x => x.WorkOrderNumber.StartsWith("WO-DEMO-") &&
                        x.Status == "COMPLETED")
            .OrderBy(x => x.WorkOrderNumber)
            .ToListAsync(cancellationToken);

        var existingRecordWorkOrderIds = await context.MaintenanceRecords
            .Select(x => x.MaintenanceWorkOrderId)
            .ToHashSetAsync(cancellationToken);

        var maintenanceRecords = new List<MaintenanceRecord>();

        for (var i = 0; i < completedWorkOrders.Count && i < 5; i++)
        {
            var workOrder = completedWorkOrders[i];

            if (existingRecordWorkOrderIds.Contains(workOrder.Id))
                continue;

            maintenanceRecords.Add(new MaintenanceRecord
            {
                Id = CreateId("A5000000", i + 1),
                MaintenanceWorkOrderId = workOrder.Id,
                PerformedByEmployeeId = null,
                MaintenanceDate = DateTimeOffset.UtcNow.AddDays(-20 + i),
                WorkDescription = $"Maintenance completed for {workOrder.Title}",
                PartsReplaced = i % 2 == 0
                    ? "Filter, oil and inspection components"
                    : "Inspection and minor replacement parts",
                LaborCost = 250000m + (i * 50000m),
                PartsCost = 200000m + (i * 75000m),
                Notes = "Demo maintenance history for portfolio"
            });
        }

        if (maintenanceRecords.Count > 0)
        {
            context.MaintenanceRecords.AddRange(maintenanceRecords);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    // HELPER METHODS
    private static T GetRequired<T>(Dictionary<string, T> dictionary, string key, string entityName)
    {
        if (!dictionary.TryGetValue(key, out var result))
        {
            throw new InvalidOperationException($"{entityName} with code '{key}' not found.");
        }
        return result;
    }

    private static Guid CreateId(string prefix, int number)
    {
        return Guid.Parse($"{prefix}-0000-0000-0000-{number:D12}");
    }
}
