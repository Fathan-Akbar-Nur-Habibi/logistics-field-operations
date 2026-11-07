using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class FieldAssignmentMemberConfiguration
    : IEntityTypeConfiguration<FieldAssignmentMember>
{
    public void Configure(EntityTypeBuilder<FieldAssignmentMember> builder)
    {
        builder.ToTable("field_assignment_members");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Responsibility)
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue("ASSIGNED");

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke field assignment
        builder.HasOne(x => x.FieldAssignment)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.FieldAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi ke karyawan
        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Satu karyawan hanya satu kali
        // dalam satu penugasan.
        builder.HasIndex(x => new
        {
            x.FieldAssignmentId,
            x.EmployeeId
        }).IsUnique();

        builder.HasIndex(x => x.EmployeeId);

        builder.HasIndex(x => x.Status);
    }
}