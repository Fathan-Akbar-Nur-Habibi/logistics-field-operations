using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class AssetMovementConfiguration : IEntityTypeConfiguration<AssetMovement>
{
    public void Configure(EntityTypeBuilder<AssetMovement> builder)
    {
        builder.ToTable("asset_movements");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke aset
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Movements)
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        // Lokasi asal
        builder.HasOne(x => x.FromLocation)
            .WithMany()
            .HasForeignKey(x => x.FromLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Lokasi tujuan
        builder.HasOne(x => x.ToLocation)
            .WithMany()
            .HasForeignKey(x => x.ToLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // User yang melakukan perpindahan
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.MovedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.AssetId);

        builder.HasIndex(x => x.FromLocationId);

        builder.HasIndex(x => x.ToLocationId);

        builder.HasIndex(x => x.MovementDate);
    }
}
