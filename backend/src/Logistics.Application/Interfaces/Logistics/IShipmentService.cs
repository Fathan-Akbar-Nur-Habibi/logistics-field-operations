using Logistics.Application.DTOs.Logistics.Shipment;

namespace Logistics.Application.Interfaces.Logistics;

public interface IShipmentService
{
    Task<List<ShipmentResponse>> GetShipmentsAsync(string? search,
        string? status, Guid? originLocationId, Guid? destinationLocationId,
        Guid? materialRequestId, DateTimeOffset? shippedAtFrom,
        DateTimeOffset? shippedAtTo, CancellationToken cancellationToken = default);

    Task<ShipmentResponse?> GetShipmentByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> CreateShipmentAsync(CreateShipmentRequest request,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> UpdateShipmentAsync(Guid id,
        UpdateShipmentRequest request,CancellationToken cancellationToken = default);

    Task<ShipmentResponse> UpdateShipmentStatusAsync(Guid id,
        UpdateShipmentStatusRequest request,CancellationToken cancellationToken = default);

    Task<ShipmentItemResponse> AddShipmentItemAsync(Guid shipmentId,
        Guid itemId,decimal quantity,string? notes,
        CancellationToken cancellationToken = default);
}
