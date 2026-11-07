using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class CrewScheduleConfiguration : IEntityTypeConfiguration<CrewSchedule>
{
    public void Configure(EntityTypeBuilder<CrewSchedule> builder)
    {
        builder.ToTable("crew_schedules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue("SCHEDULED");

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke karyawan
        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi ke shift
        builder.HasOne(x => x.Shift)
            .WithMany(x => x.CrewSchedules)
            .HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi ke lokasi kerja
        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Mencegah karyawan dijadwalkan dua kali
        // pada shift yang sama di tanggal yang sama.
        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.ShiftId,
            x.WorkDate
        }).IsUnique();

        builder.HasIndex(x => x.ShiftId);

        builder.HasIndex(x => x.LocationId);

        builder.HasIndex(x => x.WorkDate);

        builder.HasIndex(x => x.Status);
    }
}