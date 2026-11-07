
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Asset : BaseEntity
{
    public string AssetCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid AssetCategoryId { get; set; }

    public Guid LocationId { get; set; }

    public string? SerialNumber { get; set; }

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public decimal? PurchasePrice { get; set; }

    public DateOnly? WarrantyExpiryDate { get; set; }

    public string Status { get; set; } = "AVAILABLE";

    public string? QRCode { get; set; }

    public string? Notes { get; set; }

    // Navigation Properties
    public AssetCategory AssetCategory { get; set; } = null!;

    public Location Location { get; set; } = null!;

    public ICollection<AssetMovement> Movements { get; set; }
        = new List<AssetMovement>();

    public ICollection<MaintenancePlan> MaintenancePlans { get; set; }
        = new List<MaintenancePlan>();

    public ICollection<MaintenanceWorkOrder> WorkOrders { get; set; }
        = new List<MaintenanceWorkOrder>();
}
