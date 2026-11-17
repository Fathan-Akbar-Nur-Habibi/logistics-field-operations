using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Logistics.Shipment;

public class UpdateShipmentStatusRequest
{
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? LocationName { get; set; }

    public DateTimeOffset? EventDate { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}
