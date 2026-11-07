
using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Location : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Province { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsActive { get; set; } = true;
}
