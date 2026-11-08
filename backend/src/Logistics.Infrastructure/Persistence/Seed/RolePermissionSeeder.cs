
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Seed;

public static class RolePermissionSeeder
{
    public static async Task SeedAsync(
        LogisticsDbContext context,
        CancellationToken cancellationToken = default)
    {
        // Ambil seluruh role yang sudah di-seed.
        var roles = await context.Roles
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Id,
                cancellationToken);

        // Ambil seluruh permission yang sudah di-seed.
        var permissions = await context.Permissions
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Id,
                cancellationToken);

        // Validasi agar seeder tidak berjalan
        // jika RoleSeeder atau PermissionSeeder belum dijalankan.
        var requiredRoles = new[]
        {
            "SUPER_ADMIN",
            "ADMIN",
            "MANAGER",
            "OPERATOR",
            "FIELD_OFFICER"
        };

        foreach (var roleCode in requiredRoles)
        {
            if (!roles.ContainsKey(roleCode))
            {
                throw new InvalidOperationException(
                    $"Role '{roleCode}' belum tersedia. Jalankan RoleSeeder terlebih dahulu.");
            }
        }

        var existingRolePermissions = await context.RolePermissions
            .Select(x => new
            {
                x.RoleId,
                x.PermissionId
            })
            .ToListAsync(cancellationToken);

        var existingSet = existingRolePermissions
            .Select(x => (x.RoleId, x.PermissionId))
            .ToHashSet();

        var assignments = new List<RolePermission>();

        void Assign(string roleCode, IEnumerable<string> permissionCodes)
        {
            var roleId = roles[roleCode];

            foreach (var permissionCode in permissionCodes.Distinct())
            {
                if (!permissions.TryGetValue(permissionCode, out var permissionId))
                {
                    throw new InvalidOperationException(
                        $"Permission '{permissionCode}' belum tersedia. Jalankan PermissionSeeder terlebih dahulu.");
                }

                var key = (roleId, permissionId);

                if (existingSet.Add(key))
                {
                    assignments.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    });
                }
            }
        }

        var allPermissionCodes = permissions.Keys.ToList();

        // 1. SUPER ADMIN
        // Memiliki seluruh permission.
        Assign("SUPER_ADMIN", allPermissionCodes);

        // 2. ADMIN
        // Seluruh permission kecuali pengelolaan definisi permission.
        Assign(
            "ADMIN",
            allPermissionCodes.Where(x =>
                x != "USER_ACCESS.MANAGE_PERMISSION"));

        // 3. MANAGER
        Assign("MANAGER", new[]
        {
            "DASHBOARD.VIEW",
            "DASHBOARD.EXPORT",

            "INVENTORY.VIEW",
            "INVENTORY.APPROVE",
            "INVENTORY.EXPORT",

            "ASSET.VIEW",

            "MAINTENANCE.VIEW",
            "MAINTENANCE.CREATE",
            "MAINTENANCE.UPDATE",
            "MAINTENANCE.COMPLETE",

            "CREW.VIEW",
            "CREW.CREATE",
            "CREW.UPDATE",
            "CREW.ASSIGN",

            "LOGISTICS.VIEW",
            "LOGISTICS.CREATE",
            "LOGISTICS.UPDATE",
            "LOGISTICS.APPROVE",
            "LOGISTICS.SHIP",
            "LOGISTICS.TRACK",

            "USER_ACCESS.VIEW"
        });

        // 4. OPERATOR
        Assign("OPERATOR", new[]
        {
            "DASHBOARD.VIEW",

            "INVENTORY.VIEW",
            "INVENTORY.CREATE",
            "INVENTORY.UPDATE",

            "ASSET.VIEW",
            "ASSET.CREATE",
            "ASSET.UPDATE",

            "MAINTENANCE.VIEW",
            "MAINTENANCE.CREATE",
            "MAINTENANCE.UPDATE",
            "MAINTENANCE.COMPLETE",

            "CREW.VIEW",
            "CREW.UPDATE",

            "LOGISTICS.VIEW",
            "LOGISTICS.CREATE",
            "LOGISTICS.UPDATE",
            "LOGISTICS.TRACK"
        });

        // 5. FIELD OFFICER
        Assign("FIELD_OFFICER", new[]
        {
            "DASHBOARD.VIEW",

            "INVENTORY.VIEW",

            "ASSET.VIEW",

            "MAINTENANCE.VIEW",
            "MAINTENANCE.UPDATE",
            "MAINTENANCE.COMPLETE",

            "CREW.VIEW",

            "LOGISTICS.VIEW",
            "LOGISTICS.TRACK"
        });

        if (assignments.Count == 0)
        {
            return;
        }

        await context.RolePermissions.AddRangeAsync(
            assignments,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
