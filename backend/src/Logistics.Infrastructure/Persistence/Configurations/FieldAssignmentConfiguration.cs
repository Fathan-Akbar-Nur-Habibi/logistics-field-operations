using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class FieldAssignmentConfiguration
    : IEntityTypeConfiguration<FieldAssignment>
{
    public void Configure(EntityTypeBuilder<FieldAssignment> builder)
    {
        builder.ToTable("field_assignments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.AssignmentNumber)
            .HasMaxLength(50)
            .IsRequired();

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
            .HasDefaultValue("PLANNED");

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke lokasi penugasan
        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // User pembuat penugasan (opsional)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relasi ke anggota penugasan
        builder.HasMany(x => x.Members)
            .WithOne(x => x.FieldAssignment)
            .HasForeignKey(x => x.FieldAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AssignmentNumber)
            .IsUnique();

        builder.HasIndex(x => x.LocationId);

        builder.HasIndex(x => x.StartDate);

        builder.HasIndex(x => x.EndDate);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Priority);
    }
}