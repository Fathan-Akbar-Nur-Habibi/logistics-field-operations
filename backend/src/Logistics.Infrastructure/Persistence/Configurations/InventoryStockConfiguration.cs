using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
{
    public void Configure(EntityTypeBuilder<InventoryStock> builder)
    {
        builder.ToTable("inventory_stocks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.ReservedQuantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke barang
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi ke lokasi penyimpanan
        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Satu barang hanya memiliki satu saldo stok
        // pada satu lokasi.
        builder.HasIndex(x => new
        {
            x.ItemId,
            x.LocationId
        }).IsUnique();

        builder.HasIndex(x => x.LocationId);
    }
}