using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Inventory;

public class UpdateItemRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public Guid ItemCategoryId { get; set; }

    [Required]
    public Guid UnitOfMeasureId { get; set; }

    [Range(typeof(decimal), "0", "999999999999999.999")]
    public decimal MinimumStock { get; set; }

    [Range(typeof(decimal), "0", "999999999999999.99")]
    public decimal? UnitPrice { get; set; }

    public bool IsActive { get; set; } = true;
}