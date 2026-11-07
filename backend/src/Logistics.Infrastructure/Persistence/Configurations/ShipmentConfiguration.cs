using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("shipments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ShipmentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(x => x.CarrierName)
            .HasMaxLength(150);

        builder.Property(x => x.Status)
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue("PREPARING");

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi opsional ke material request.
        // Konfigurasi inverse-nya sudah ditentukan
        // pada MaterialRequestConfiguration.
        builder.HasOne(x => x.MaterialRequest)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.MaterialRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Lokasi asal pengiriman
        builder.HasOne(x => x.OriginLocation)
            .WithMany()
            .HasForeignKey(x => x.OriginLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Lokasi tujuan pengiriman
        builder.HasOne(x => x.DestinationLocation)
            .WithMany()
            .HasForeignKey(x => x.DestinationLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // User pembuat shipment
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Detail barang dalam shipment
        builder.HasMany(x => x.Items)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Riwayat tracking shipment
        builder.HasMany(x => x.TrackingEvents)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ShipmentNumber)
            .IsUnique();

        builder.HasIndex(x => x.TrackingNumber)
            .IsUnique()
            .HasFilter("\"TrackingNumber\" IS NOT NULL");

        builder.HasIndex(x => x.MaterialRequestId);

        builder.HasIndex(x => x.OriginLocationId);

        builder.HasIndex(x => x.DestinationLocationId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.ShippedAt);

        builder.HasIndex(x => x.EstimatedArrivalAt);
    }
}