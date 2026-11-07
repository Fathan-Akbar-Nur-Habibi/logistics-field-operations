using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class MaterialRequestConfiguration
    : IEntityTypeConfiguration<MaterialRequest>
{
    public void Configure(EntityTypeBuilder<MaterialRequest> builder)
    {
        builder.ToTable("material_requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.RequestNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("MEDIUM");

        builder.Property(x => x.Status)
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue("PENDING");

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Lokasi yang mengajukan permintaan
        builder.HasOne(x => x.RequestingLocation)
            .WithMany()
            .HasForeignKey(x => x.RequestingLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Lokasi tujuan penerimaan barang
        builder.HasOne(x => x.DestinationLocation)
            .WithMany()
            .HasForeignKey(x => x.DestinationLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // User yang mengajukan permintaan
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.RequestedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // User yang menyetujui permintaan
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.ApprovedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Detail barang yang diminta
        builder.HasMany(x => x.Items)
            .WithOne(x => x.MaterialRequest)
            .HasForeignKey(x => x.MaterialRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        // Shipment yang berasal dari permintaan ini
        builder.HasMany(x => x.Shipments)
            .WithOne(x => x.MaterialRequest)
            .HasForeignKey(x => x.MaterialRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.RequestNumber)
            .IsUnique();

        builder.HasIndex(x => x.RequestingLocationId);

        builder.HasIndex(x => x.DestinationLocationId);

        builder.HasIndex(x => x.RequestDate);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Priority);
    }
}