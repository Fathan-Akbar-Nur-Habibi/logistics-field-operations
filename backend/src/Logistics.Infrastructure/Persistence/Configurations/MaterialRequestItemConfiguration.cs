using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class MaterialRequestItemConfiguration
    : IEntityTypeConfiguration<MaterialRequestItem>
{
    public void Configure(EntityTypeBuilder<MaterialRequestItem> builder)
    {
        builder.ToTable("material_request_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.RequestedQuantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.ApprovedQuantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke header permintaan
        builder.HasOne(x => x.MaterialRequest)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.MaterialRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi ke master barang
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Satu barang hanya satu kali dalam satu permintaan.
        builder.HasIndex(x => new
        {
            x.MaterialRequestId,
            x.ItemId
        }).IsUnique();

        builder.HasIndex(x => x.ItemId);
    }
}