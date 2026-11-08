
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Seed;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        LogisticsDbContext context,
        CancellationToken cancellationToken = default)
    {
        var permissions = new List<Permission>
        {
            // DASHBOARD & ANALYTICS
            Create(1, "DASHBOARD.VIEW", "View Dashboard",
                "Dashboard", "Melihat dashboard dan ringkasan operasional."),

            Create(2, "DASHBOARD.EXPORT", "Export Dashboard",
                "Dashboard", "Mengunduh laporan dashboard."),

            // INVENTORY MANAGEMENT
            Create(3, "INVENTORY.VIEW", "View Inventory",
                "Inventory", "Melihat data inventaris."),

            Create(4, "INVENTORY.CREATE", "Create Inventory",
                "Inventory", "Menambahkan data barang dan transaksi inventaris."),

            Create(5, "INVENTORY.UPDATE", "Update Inventory",
                "Inventory", "Mengubah data inventaris."),

            Create(6, "INVENTORY.DELETE", "Delete Inventory",
                "Inventory", "Menghapus data inventaris."),

            Create(7, "INVENTORY.APPROVE", "Approve Stock Opname",
                "Inventory", "Menyetujui hasil stock opname."),

            Create(8, "INVENTORY.EXPORT", "Export Inventory",
                "Inventory", "Mengunduh laporan inventaris."),

            // ASSET MANAGEMENT
            Create(9, "ASSET.VIEW", "View Assets",
                "Asset", "Melihat data aset."),

            Create(10, "ASSET.CREATE", "Create Asset",
                "Asset", "Menambahkan aset baru."),

            Create(11, "ASSET.UPDATE", "Update Asset",
                "Asset", "Mengubah data aset."),

            Create(12, "ASSET.DELETE", "Delete Asset",
                "Asset", "Menghapus data aset."),

            // MAINTENANCE MANAGEMENT
            Create(13, "MAINTENANCE.VIEW", "View Maintenance",
                "Maintenance", "Melihat jadwal dan riwayat pemeliharaan."),

            Create(14, "MAINTENANCE.CREATE", "Create Maintenance",
                "Maintenance", "Membuat jadwal atau work order pemeliharaan."),

            Create(15, "MAINTENANCE.UPDATE", "Update Maintenance",
                "Maintenance", "Mengubah data pemeliharaan."),

            Create(16, "MAINTENANCE.DELETE", "Delete Maintenance",
                "Maintenance", "Menghapus data pemeliharaan."),

            Create(17, "MAINTENANCE.COMPLETE", "Complete Maintenance",
                "Maintenance", "Menyelesaikan work order pemeliharaan."),

            // CREW & FIELD OPERATIONS
            Create(18, "CREW.VIEW", "View Crew",
                "Crew", "Melihat data karyawan dan jadwal kerja."),

            Create(19, "CREW.CREATE", "Create Crew",
                "Crew", "Menambahkan data karyawan dan jadwal kerja."),

            Create(20, "CREW.UPDATE", "Update Crew",
                "Crew", "Mengubah data karyawan dan jadwal kerja."),

            Create(21, "CREW.DELETE", "Delete Crew",
                "Crew", "Menghapus data karyawan dan jadwal kerja."),

            Create(22, "CREW.ASSIGN", "Assign Field Operation",
                "Crew", "Menugaskan karyawan ke pekerjaan lapangan."),

            // LOGISTICS & TRACKING
            Create(23, "LOGISTICS.VIEW", "View Logistics",
                "Logistics", "Melihat permintaan material dan pengiriman."),

            Create(24, "LOGISTICS.CREATE", "Create Logistics",
                "Logistics", "Membuat permintaan material dan pengiriman."),

            Create(25, "LOGISTICS.UPDATE", "Update Logistics",
                "Logistics", "Mengubah data logistik."),

            Create(26, "LOGISTICS.DELETE", "Delete Logistics",
                "Logistics", "Menghapus data logistik."),

            Create(27, "LOGISTICS.APPROVE", "Approve Material Request",
                "Logistics", "Menyetujui permintaan material."),

            Create(28, "LOGISTICS.SHIP", "Process Shipment",
                "Logistics", "Memproses pengiriman barang."),

            Create(29, "LOGISTICS.TRACK", "Track Shipment",
                "Logistics", "Melihat dan memperbarui informasi tracking."),

            // USER & ACCESS MANAGEMENT
            Create(30, "USER_ACCESS.VIEW", "View Users",
                "UserAccess", "Melihat data pengguna dan role."),

            Create(31, "USER_ACCESS.CREATE", "Create User",
                "UserAccess", "Membuat akun pengguna."),

            Create(32, "USER_ACCESS.UPDATE", "Update User",
                "UserAccess", "Mengubah data pengguna."),

            Create(33, "USER_ACCESS.DELETE", "Delete User",
                "UserAccess", "Menghapus atau menonaktifkan pengguna."),

            Create(34, "USER_ACCESS.ASSIGN_ROLE", "Assign User Role",
                "UserAccess", "Mengatur role pengguna."),

            Create(35, "USER_ACCESS.MANAGE_PERMISSION", "Manage Permissions",
                "UserAccess", "Mengatur hak akses role.")
        };

        var existingCodes = await context.Permissions
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var newPermissions = permissions
            .Where(x => !existingCodes.Contains(x.Code))
            .ToList();

        if (newPermissions.Count == 0)
        {
            return;
        }

        await context.Permissions.AddRangeAsync(
            newPermissions,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static Permission Create(
        int number,
        string code,
        string name,
        string module,
        string description)
    {
        return new Permission
        {
            Id = Guid.Parse(
                $"20000000-0000-0000-0000-{number:000000000000}"),

            Code = code,
            Name = name,
            Module = module,
            Description = description
        };
    }
}
