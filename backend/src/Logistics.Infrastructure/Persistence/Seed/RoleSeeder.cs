
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        LogisticsDbContext context,
        CancellationToken cancellationToken = default)
    {
        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Code = "SUPER_ADMIN",
                Name = "Super Administrator",
                Description = "Memiliki akses penuh ke seluruh sistem.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Code = "ADMIN",
                Name = "Administrator",
                Description = "Mengelola operasional dan konfigurasi aplikasi.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Code = "MANAGER",
                Name = "Manager",
                Description = "Memantau operasional dan menyetujui aktivitas tertentu.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Code = "OPERATOR",
                Name = "Operator",
                Description = "Mengelola aktivitas operasional dan inventaris.",
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Code = "FIELD_OFFICER",
                Name = "Field Officer",
                Description = "Melaksanakan tugas operasional di lapangan.",
                IsActive = true
            }
        };

        var existingRoleCodes = await context.Roles
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var newRoles = roles
            .Where(x => !existingRoleCodes.Contains(x.Code))
            .ToList();

        if (newRoles.Count == 0)
        {
            return;
        }

        await context.Roles.AddRangeAsync(
            newRoles,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
