using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class MaintenanceWorkOrderConfiguration
    : IEntityTypeConfiguration<MaintenanceWorkOrder>
{
    public void Configure(EntityTypeBuilder<MaintenanceWorkOrder> builder)
    {
        builder.ToTable("maintenance_work_orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.WorkOrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.MaintenanceType)
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue("PREVENTIVE");

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Priority)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("MEDIUM");

        builder.Property(x => x.Status)
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue("OPEN");

        builder.Property(x => x.EstimatedCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.ActualCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke aset
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.WorkOrders)
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi ke maintenance plan (opsional)
        builder.HasOne(x => x.MaintenancePlan)
            .WithMany()
            .HasForeignKey(x => x.MaintenancePlanId)
            .OnDelete(DeleteBehavior.SetNull);

        // Karyawan yang ditugaskan (opsional)
        builder.HasOne(x => x.AssignedToEmployee)
            .WithMany()
            .HasForeignKey(x => x.AssignedToEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // User pembuat work order (opsional)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relasi ke catatan maintenance
        builder.HasMany(x => x.Records)
            .WithOne(x => x.MaintenanceWorkOrder)
            .HasForeignKey(x => x.MaintenanceWorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.WorkOrderNumber)
            .IsUnique();

        builder.HasIndex(x => x.AssetId);

        builder.HasIndex(x => x.MaintenancePlanId);

        builder.HasIndex(x => x.AssignedToEmployeeId);

        builder.HasIndex(x => x.ScheduledDate);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Priority);
    }
}