
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Seed;

public static class MasterDataSeeder
{
    public static async Task SeedAsync(
        LogisticsDbContext context,
        CancellationToken cancellationToken = default)
    {
        // =====================================
        // 1. LOCATION
        // =====================================

        var locations = new List<Location>
        {
            new()
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Code = "WH-JKT",
                Name = "Jakarta Main Warehouse",
                Description = "Gudang utama operasional Jakarta.",
                City = "Jakarta",
                Province = "DKI Jakarta",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Code = "DC-BKS",
                Name = "Bekasi Distribution Center",
                Description = "Pusat distribusi wilayah Bekasi.",
                City = "Bekasi",
                Province = "Jawa Barat",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                Code = "HUB-BDG",
                Name = "Bandung Hub",
                Description = "Hub operasional wilayah Bandung.",
                City = "Bandung",
                Province = "Jawa Barat",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                Code = "HUB-SBY",
                Name = "Surabaya Hub",
                Description = "Hub operasional wilayah Surabaya.",
                City = "Surabaya",
                Province = "Jawa Timur",
                IsActive = true
            }
        };

        var existingLocationCodes = await context.Locations
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var newLocations = locations
            .Where(x => !existingLocationCodes.Contains(x.Code))
            .ToList();

        // =====================================
        // 2. ITEM CATEGORY
        // =====================================

        var itemCategories = new List<ItemCategory>
        {
            new()
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                Code = "ELEC",
                Name = "Electronic Equipment",
                Description = "Peralatan elektronik dan kelistrikan.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                Code = "OFFICE",
                Name = "Office Supplies",
                Description = "Perlengkapan kantor.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000003"),
                Code = "SAFETY",
                Name = "Safety Equipment",
                Description = "Perlengkapan keselamatan kerja.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000004"),
                Code = "TOOLS",
                Name = "Tools and Equipment",
                Description = "Peralatan kerja dan perkakas.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000005"),
                Code = "SPAREPART",
                Name = "Spare Parts",
                Description = "Suku cadang kendaraan dan mesin.",
                IsActive = true
            }
        };

        var existingItemCategoryCodes = await context.ItemCategories
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var newItemCategories = itemCategories
            .Where(x => !existingItemCategoryCodes.Contains(x.Code))
            .ToList();

        // =====================================
        // 3. UNIT OF MEASURE
        // =====================================

        var units = new List<UnitOfMeasure>
        {
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                Code = "PCS",
                Name = "Pieces",
                Description = "Satuan buah.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                Code = "BOX",
                Name = "Box",
                Description = "Satuan kotak.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                Code = "SET",
                Name = "Set",
                Description = "Satuan set.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000004"),
                Code = "UNIT",
                Name = "Unit",
                Description = "Satuan unit.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000005"),
                Code = "KG",
                Name = "Kilogram",
                Description = "Satuan berat kilogram.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000006"),
                Code = "LITER",
                Name = "Liter",
                Description = "Satuan volume liter.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000007"),
                Code = "METER",
                Name = "Meter",
                Description = "Satuan panjang meter.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000008"),
                Code = "ROLL",
                Name = "Roll",
                Description = "Satuan gulungan.",
                IsActive = true
            }
        };

        var existingUnitCodes = await context.UnitsOfMeasure
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var newUnits = units
            .Where(x => !existingUnitCodes.Contains(x.Code))
            .ToList();

        // =====================================
        // 4. ASSET CATEGORY
        // =====================================

        var assetCategories = new List<AssetCategory>
        {
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                Code = "VEHICLE",
                Name = "Vehicles",
                Description = "Kendaraan operasional.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000002"),
                Code = "MACHINERY",
                Name = "Machinery",
                Description = "Mesin dan peralatan mekanis.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000003"),
                Code = "IT",
                Name = "IT Equipment",
                Description = "Komputer dan perangkat teknologi informasi.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000004"),
                Code = "OFFICE_ASSET",
                Name = "Office Assets",
                Description = "Peralatan dan perlengkapan kantor.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000005"),
                Code = "WAREHOUSE",
                Name = "Warehouse Equipment",
                Description = "Peralatan operasional gudang.",
                IsActive = true
            }
        };

        var existingAssetCategoryCodes = await context.AssetCategories
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var newAssetCategories = assetCategories
            .Where(x => !existingAssetCategoryCodes.Contains(x.Code))
            .ToList();

        // =====================================
        // 5. SHIFT
        // =====================================

        var shifts = new List<Shift>
        {
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                Code = "SHIFT-1",
                Name = "Morning Shift",
                StartTime = new TimeOnly(6, 0),
                EndTime = new TimeOnly(14, 0),
                Description = "Shift pagi pukul 06:00 - 14:00.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                Code = "SHIFT-2",
                Name = "Afternoon Shift",
                StartTime = new TimeOnly(14, 0),
                EndTime = new TimeOnly(22, 0),
                Description = "Shift sore pukul 14:00 - 22:00.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000003"),
                Code = "SHIFT-3",
                Name = "Night Shift",
                StartTime = new TimeOnly(22, 0),
                EndTime = new TimeOnly(6, 0),
                Description = "Shift malam pukul 22:00 - 06:00.",
                IsActive = true
            }
        };

        var existingShiftCodes = await context.Shifts
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var newShifts = shifts
            .Where(x => !existingShiftCodes.Contains(x.Code))
            .ToList();

        // =====================================
        // SAVE ALL SEED DATA
        // =====================================

        await context.Locations.AddRangeAsync(
            newLocations, cancellationToken);

        await context.ItemCategories.AddRangeAsync(
            newItemCategories, cancellationToken);

        await context.UnitsOfMeasure.AddRangeAsync(
            newUnits, cancellationToken);

        await context.AssetCategories.AddRangeAsync(
            newAssetCategories, cancellationToken);

        await context.Shifts.AddRangeAsync(
            newShifts, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
