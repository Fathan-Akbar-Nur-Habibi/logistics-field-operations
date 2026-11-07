using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Persistence.Configurations;

public class StockOpnameDetailConfiguration : IEntityTypeConfiguration<StockOpnameDetail>
{
    public void Configure(EntityTypeBuilder<StockOpnameDetail> builder)
    {
        builder.ToTable("stock_opname_details");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.SystemQuantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.PhysicalQuantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Relasi ke barang
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi ke header stock opname
        builder.HasOne(x => x.StockOpname)
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.StockOpnameId)
            .OnDelete(DeleteBehavior.Restrict);

        // Satu barang hanya satu kali dalam satu stock opname.
        builder.HasIndex(x => new
        {
            x.StockOpnameId,
            x.ItemId
        }).IsUnique();

        builder.HasIndex(x => x.ItemId);
    }
}