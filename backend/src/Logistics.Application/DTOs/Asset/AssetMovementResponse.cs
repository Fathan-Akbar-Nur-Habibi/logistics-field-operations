namespace Logistics.Application.DTOs.Asset;

public class AssetMovementResponse
{
    public Guid Id { get; set; }

    public Guid AssetId { get; set; }

    public string AssetCode { get; set; } = string.Empty;

    public string AssetName { get; set; } = string.Empty;

    public Guid FromLocationId { get; set; }

    public string FromLocationCode { get; set; } = string.Empty;

    public string FromLocationName { get; set; } = string.Empty;

    public Guid ToLocationId { get; set; }

    public string ToLocationCode { get; set; } = string.Empty;

    public string ToLocationName { get; set; } = string.Empty;

    public DateTimeOffset MovementDate { get; set; }

    public string? Reason { get; set; }

    public Guid? MovedByUserId { get; set; }
}