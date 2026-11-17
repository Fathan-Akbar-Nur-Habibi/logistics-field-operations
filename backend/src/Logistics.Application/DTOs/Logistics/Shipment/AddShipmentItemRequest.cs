using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Logistics.Shipment;

public class AddShipmentItemRequest
{
    [Required]
    public Guid ItemId { get; set; }

    [Range(
        typeof(decimal),
        "0.001",
        "999999999999999.999",
        ErrorMessage = "Quantity must be greater than zero."
    )]
    public decimal Quantity { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
