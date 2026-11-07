using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class MaintenanceRecordConfiguration
    : IEntityTypeConfiguration<MaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
    {
        builder.ToTable("maintenance_records");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.WorkDescription)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.PartsReplaced)
            .HasMaxLength(1000);

        builder.Property(x => x.LaborCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.PartsCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke work order
        builder.HasOne(x => x.MaintenanceWorkOrder)
            .WithMany(x => x.Records)
            .HasForeignKey(x => x.MaintenanceWorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Karyawan pelaksana maintenance (opsional)
        builder.HasOne(x => x.PerformedByEmployee)
            .WithMany()
            .HasForeignKey(x => x.PerformedByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.MaintenanceWorkOrderId);

        builder.HasIndex(x => x.PerformedByEmployeeId);

        builder.HasIndex(x => x.MaintenanceDate);
    }
}