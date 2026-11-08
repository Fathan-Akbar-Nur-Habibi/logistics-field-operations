
using Logistics.Infrastructure.Persistence;

namespace Logistics.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        LogisticsDbContext context,
        CancellationToken cancellationToken = default)
    {
        // 1. Seed Role
        await RoleSeeder.SeedAsync(context,cancellationToken);
        // 2. Seed Permission
        await PermissionSeeder.SeedAsync(context,cancellationToken);
        // 3. Seed Role-Permission
        await RolePermissionSeeder.SeedAsync(context,cancellationToken);
        // 4. Master Data
        await MasterDataSeeder.SeedAsync(context, cancellationToken);
         // 5. Operational Data
        await OperationalDataSeeder.SeedAsync(context, cancellationToken);
        // 6. Operational History Data
        await OperationalHistorySeeder.SeedAsync(context,cancellationToken);
    }
}
