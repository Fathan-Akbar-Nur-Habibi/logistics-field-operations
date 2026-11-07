
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence;

public class LogisticsDbContext : DbContext
{
    public LogisticsDbContext(
        DbContextOptions<LogisticsDbContext> options)
        : base(options)
    {
    }
    // MASTER DATA

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();

    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();

    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    // INVENTORY MANAGEMENT

    public DbSet<Item> Items => Set<Item>();

    public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();

    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    public DbSet<StockOpname> StockOpnames => Set<StockOpname>();

    public DbSet<StockOpnameDetail> StockOpnameDetails => Set<StockOpnameDetail>();


    // ASSET MANAGEMENT

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<AssetMovement> AssetMovements => Set<AssetMovement>();
    // MAINTENANCE MANAGEMENT

    public DbSet<MaintenancePlan> MaintenancePlans => Set<MaintenancePlan>();

    public DbSet<MaintenanceWorkOrder> MaintenanceWorkOrders => Set<MaintenanceWorkOrder>();

    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();

    // CREW & FIELD OPERATIONS

    public DbSet<Shift> Shifts => Set<Shift>();

    public DbSet<CrewSchedule> CrewSchedules => Set<CrewSchedule>();

    public DbSet<FieldAssignment> FieldAssignments => Set<FieldAssignment>();

    public DbSet<FieldAssignmentMember> FieldAssignmentMembers => Set<FieldAssignmentMember>();

    // LOGISTICS & TRACKING


    public DbSet<MaterialRequest> MaterialRequests => Set<MaterialRequest>();

    public DbSet<MaterialRequestItem> MaterialRequestItems => Set<MaterialRequestItem>();

    public DbSet<Shipment> Shipments => Set<Shipment>();

    public DbSet<ShipmentItem> ShipmentItems => Set<ShipmentItem>();

    public DbSet<ShipmentTrackingEvent> ShipmentTrackingEvents => Set<ShipmentTrackingEvent>();

    // USER & ACCESS MANAGEMENT

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();

    // ENTITY CONFIGURATION
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LogisticsDbContext).Assembly
        );
    }
}
