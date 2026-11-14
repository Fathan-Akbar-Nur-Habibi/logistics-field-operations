using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Asset;

public class AssetMovementRequest
{
    [Required]
    public Guid ToLocationId { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }
}