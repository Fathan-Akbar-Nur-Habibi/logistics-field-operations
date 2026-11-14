using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Asset;

public class CreateAssetRequest
{
    [Required]
    [StringLength(50)]
    public string AssetCode { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public Guid AssetCategoryId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [StringLength(100)]
    public string? SerialNumber { get; set; }

    [StringLength(100)]
    public string? Brand { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal? PurchasePrice { get; set; }

    public DateOnly? WarrantyExpiryDate { get; set; }

    [StringLength(200)]
    public string? QRCode { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}