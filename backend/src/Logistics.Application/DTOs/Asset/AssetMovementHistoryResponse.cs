namespace Logistics.Application.DTOs.Asset;

public class AssetMovementHistoryResponse
{
    public Guid Id { get; set; }

    public Guid AssetId { get; set; }

    public string AssetCode { get; set; } = string.Empty;

    public string AssetName { get; set; } = string.Empty;

    public string FromLocationName { get; set; } = string.Empty;

    public string ToLocationName { get; set; } = string.Empty;

    public DateTimeOffset MovementDate { get; set; }

    public string? Reason { get; set; }
}