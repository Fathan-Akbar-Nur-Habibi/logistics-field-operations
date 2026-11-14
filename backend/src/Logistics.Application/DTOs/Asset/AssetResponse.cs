namespace Logistics.Application.DTOs.Asset;

public class AssetResponse
{
    public Guid Id { get; set; }

    public string AssetCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid AssetCategoryId { get; set; }

    public string AssetCategoryCode { get; set; } = string.Empty;

    public string AssetCategoryName { get; set; } = string.Empty;

    public Guid LocationId { get; set; }

    public string LocationCode { get; set; } = string.Empty;

    public string LocationName { get; set; } = string.Empty;

    public string? SerialNumber { get; set; }

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public decimal? PurchasePrice { get; set; }

    public DateOnly? WarrantyExpiryDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? QRCode { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}