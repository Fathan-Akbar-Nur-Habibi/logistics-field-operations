using Logistics.Domain.Entities;

namespace Logistics.Application.Interfaces.Logistics;

public interface IShipmentRepository
{
    Task<List<Shipment>> GetShipmentsAsync(
        string? search,
        string? status,
        Guid? originLocationId,
        Guid? destinationLocationId,
        Guid? materialRequestId,
        DateTimeOffset? shippedAtFrom,
        DateTimeOffset? shippedAtTo,
        CancellationToken cancellationToken = default);

    Task<Shipment?> GetShipmentByIdAsync(Guid id,CancellationToken cancellationToken = default);

    Task<Shipment?> GetShipmentForUpdateAsync(Guid id,CancellationToken cancellationToken = default);

    Task<bool> ShipmentNumberExistsAsync(string shipmentNumber,Guid? excludeShipmentId = null,
        CancellationToken cancellationToken = default);

    Task<bool> TrackingNumberExistsAsync(string trackingNumber,Guid? excludeShipmentId = null,
        CancellationToken cancellationToken = default);

    Task<bool> IsLocationActiveAsync(Guid locationId,CancellationToken cancellationToken = default);
    Task<bool> MaterialRequestExistsAsync(Guid materialRequestId, CancellationToken cancellationToken = default);
    Task<bool> ItemExistsAsync(Guid itemId,CancellationToken cancellationToken = default);
    Task AddShipmentAsync(Shipment shipment,CancellationToken cancellationToken = default);
    Task AddShipmentItemAsync(ShipmentItem shipmentItem,CancellationToken cancellationToken = default);

    Task AddTrackingEventAsync(ShipmentTrackingEvent trackingEvent,CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
