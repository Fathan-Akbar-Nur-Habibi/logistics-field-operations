
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Seed;

public static class OperationalDataSeeder
{
    public static async Task SeedAsync(
        LogisticsDbContext context,
        CancellationToken cancellationToken = default)
    {
        // 1. LOAD MASTER DATA
        var categories = await context.ItemCategories
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Id,
                cancellationToken);

        var units = await context.UnitsOfMeasure
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Id,
                cancellationToken);

        var locations = await context.Locations
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Id,
                cancellationToken);

        var assetCategories = await context.AssetCategories
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Id,
                cancellationToken);

        // 2. SEED ITEMS
        var items = new List<Item>
        {
            CreateItem(
                1, "ITM-ELEC-001", "Laptop Operational",
                "Laptop for operational needs.",
                "ELEC", "UNIT", 2, 12500000m,
                categories, units),

            CreateItem(
                2, "ITM-ELEC-002", "Barcode Scanner",
                "Barcode scanner for inventory management.",
                "ELEC", "UNIT", 2, 1850000m,
                categories, units),

            CreateItem(
                3, "ITM-OFF-001", "A4 Paper",
                "A4 paper for office use.",
                "OFFICE", "BOX", 10, 55000m,
                categories, units),

            CreateItem(
                4, "ITM-OFF-002", "Printer Ink",
                "Printer ink for operational printers.",
                "OFFICE", "PCS", 5, 125000m,
                categories, units),

            CreateItem(
                5, "ITM-SAFE-001", "Safety Helmet",
                "Work Safety Helmet.",
                "SAFETY", "PCS", 10, 95000m,
                categories, units),

            CreateItem(
                6, "ITM-SAFE-002", "Safety Vest",
                "Work Safety Vest.",
                "SAFETY", "PCS", 10, 75000m,
                categories, units),

            CreateItem(
                7, "ITM-TOOLS-001", "Hand Pallet",
                "Manual material handling equipment.",
                "TOOLS", "UNIT", 1, 3500000m,
                categories, units),

            CreateItem(
                8, "ITM-TOOLS-002", "Tool Set",
                "Operational Tools and Equipment.",
                "TOOLS", "SET", 2, 850000m,
                categories, units),

            CreateItem(
                9, "ITM-SP-001", "Vehicle Tire",
                "Vehicle tire for operational vehicles.",
                "SPAREPART", "PCS", 4, 950000m,
                categories, units),

            CreateItem(
                10, "ITM-SP-002", "Engine Oil",
                "Engine oil for operational vehicles.",
                "SPAREPART", "LITER", 10, 85000m,
                categories, units)
        };

        var existingItemCodes = await context.Items
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var newItems = items
            .Where(x => !existingItemCodes.Contains(x.Code))
            .ToList();

        if (newItems.Count > 0)
        {
            await context.Items.AddRangeAsync(newItems,cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // 3. SEED INVENTORY STOCK
        var itemIds = await context.Items
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Id,
                cancellationToken);

        var stockData = new[]
        {
            ("ITM-ELEC-001", "WH-JKT", 15m, 2m),
            ("ITM-ELEC-002", "WH-JKT", 20m, 3m),
            ("ITM-OFF-001", "WH-JKT", 50m, 5m),
            ("ITM-OFF-002", "WH-JKT", 30m, 2m),
            ("ITM-SAFE-001", "DC-BKS", 100m, 10m),
            ("ITM-SAFE-002", "DC-BKS", 80m, 5m),
            ("ITM-TOOLS-001", "DC-BKS", 8m, 1m),
            ("ITM-TOOLS-002", "HUB-BDG", 15m, 2m),
            ("ITM-SP-001", "HUB-SBY", 25m, 4m),
            ("ITM-SP-002", "HUB-SBY", 40m, 5m)
        };

        var existingStocks = await context.InventoryStocks
            .Select(x => new
            {
                x.ItemId,
                x.LocationId
            })
            .ToListAsync(cancellationToken);

        var existingStockSet = existingStocks
            .Select(x => (x.ItemId, x.LocationId))
            .ToHashSet();

        var newStocks = stockData
            .Select((x, index) => new InventoryStock
            {
                Id = Guid.Parse(
                    $"80000000-0000-0000-0000-{index + 1:000000000000}"),

                ItemId = GetRequiredId(
                    itemIds, x.Item1, "Item"),

                LocationId = GetRequiredId(
                    locations, x.Item2, "Location"),

                Quantity = x.Item3,
                ReservedQuantity = x.Item4
            })
            .Where(x => existingStockSet.Add(
                (x.ItemId, x.LocationId)))
            .ToList();

        if (newStocks.Count > 0)
        {
            await context.InventoryStocks.AddRangeAsync(newStocks,cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // 4. SEED ASSETS
        var assets = new List<Asset>
        {
            CreateAsset(
                1, "AST-VEH-001", "Delivery Truck",
                "Freight distribution vehicles.",
                "VEHICLE", "WH-JKT",
                "Toyota", "Dyna", "AVAILABLE",
                450000000m, assetCategories, locations),

            CreateAsset(
                2, "AST-VEH-002", "Operational Pickup",
                "Field operational vehicles.",
                "VEHICLE", "DC-BKS",
                "Mitsubishi", "L300", "IN_USE",
                250000000m, assetCategories, locations),

            CreateAsset(
                3, "AST-VEH-003", "Forklift",
                "Warehouse material handling vehicles.",
                "VEHICLE", "WH-JKT",
                "Toyota", "Forklift", "AVAILABLE",
                180000000m, assetCategories, locations),

            CreateAsset(
                4, "AST-MCH-001", "Generator",
                "Backup power generator.",
                "MACHINERY", "WH-JKT",
                "Honda", "Generator", "AVAILABLE",
                25000000m, assetCategories, locations),

            CreateAsset(
                5, "AST-MCH-002", "Air Compressor",
                "Air compressor machine.",
                "MACHINERY", "DC-BKS",
                "Atlas Copco", "Compressor", "MAINTENANCE",
                45000000m, assetCategories, locations),

            CreateAsset(
                6, "AST-IT-001", "Office Desktop",
                "Office desktop computer.",
                "IT", "HUB-BDG",
                "Dell", "OptiPlex", "IN_USE",
                12000000m, assetCategories, locations),

            CreateAsset(
                7, "AST-IT-002", "Network Server",
                "Network server.",
                "IT", "WH-JKT",
                "HP", "ProLiant", "IN_USE",
                35000000m, assetCategories, locations),

            CreateAsset(
                8, "AST-OFF-001", "Office Workstation",
                "Office workstations.",
                "OFFICE_ASSET", "HUB-BDG",
                "Local", "Workstation", "AVAILABLE",
                3500000m, assetCategories, locations),

            CreateAsset(
                9, "AST-WH-001", "Electric Pallet Stacker",
                "Warehouse material handling equipment.",
                "WAREHOUSE", "DC-BKS",
                "Jungheinrich", "EJC", "AVAILABLE",
                85000000m, assetCategories, locations),

            CreateAsset(
                10, "AST-WH-002", "Industrial Shelving",
                "Industrial storage racks.",
                "WAREHOUSE", "HUB-SBY",
                "Local", "Heavy Duty Rack", "AVAILABLE",
                15000000m, assetCategories, locations)
        };

        var existingAssetCodes = await context.Assets
            .Select(x => x.AssetCode)
            .ToListAsync(cancellationToken);

        var newAssets = assets
            .Where(x => !existingAssetCodes.Contains(x.AssetCode))
            .ToList();

        if (newAssets.Count > 0)
        {
            await context.Assets.AddRangeAsync(
                newAssets,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    // HELPER: CREATE ITEM
    private static Item CreateItem(
        int number,
        string code,
        string name,
        string description,
        string categoryCode,
        string unitCode,
        decimal minimumStock,
        decimal unitPrice,
        Dictionary<string, Guid> categories,
        Dictionary<string, Guid> units)
    {
        return new Item
        {
            Id = Guid.Parse(
                $"81000000-0000-0000-0000-{number:000000000000}"),

            Code = code,
            Name = name,
            Description = description,

            ItemCategoryId = GetRequiredId(
                categories, categoryCode, "ItemCategory"),

            UnitOfMeasureId = GetRequiredId(
                units, unitCode, "UnitOfMeasure"),

            MinimumStock = minimumStock,
            UnitPrice = unitPrice,
            IsActive = true
        };
    }

    // HELPER: CREATE ASSET
    private static Asset CreateAsset(
        int number,
        string code,
        string name,
        string description,
        string categoryCode,
        string locationCode,
        string brand,
        string model,
        string status,
        decimal purchasePrice,
        Dictionary<string, Guid> assetCategories,
        Dictionary<string, Guid> locations)
    {
        return new Asset
        {
            Id = Guid.Parse(
                $"90000000-0000-0000-0000-{number:000000000000}"),

            AssetCode = code,
            Name = name,
            Description = description,

            AssetCategoryId = GetRequiredId(
                assetCategories, categoryCode, "AssetCategory"),

            LocationId = GetRequiredId(
                locations, locationCode, "Location"),

            Brand = brand,
            Model = model,
            SerialNumber = $"SN-DEMO-{number:000}",

            PurchaseDate = new DateOnly(2025, 1, 15),
            PurchasePrice = purchasePrice,
            WarrantyExpiryDate = new DateOnly(2027, 1, 15),

            Status = status,
            QRCode = $"QR-{code}",
            Notes = "Demo data for portfolio."
        };
    }

    // HELPER: VALIDATE MASTER DATA
    private static Guid GetRequiredId(Dictionary<string, Guid> dictionary,string code,string entityName)
    {
        if (!dictionary.TryGetValue(code, out var id))
        {
            throw new InvalidOperationException(
                $"{entityName} with Code '{code}' Not found. " +
                "Make sure MasterDataSeeder is running.");
        }

        return id;
    }
}
