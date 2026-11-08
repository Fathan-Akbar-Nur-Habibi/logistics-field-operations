
using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Inventory;

public class CreateStockTransactionRequest
{
    [Required]
    public Guid ItemId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    /*
    [Required]
    [RegularExpression(
        "^(IN|OUT)$",
        ErrorMessage = "TransactionType must be either IN or OUT.")]
    */
    public string TransactionType { get; set; } = string.Empty;

    [Range(
        typeof(decimal),
        "0.001",
        "999999999999999.999",
        ErrorMessage = "Quantity must be greater than 0.")]
    public decimal Quantity { get; set; }

    [Range(
        typeof(decimal),
        "0",
        "999999999999999.99",
        ErrorMessage = "UnitPrice must not be negative.")]
    public decimal UnitPrice { get; set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
