using Logistics.Application.DTOs.Logistics.Shipment;
using Logistics.Application.Interfaces.Logistics;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services.Logistics;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _repository;

    private static readonly HashSet<string> AllowedStatuses =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "PREPARING",
            "IN_TRANSIT",
            "DELAYED",
            "DELIVERED",
            "CANCELLED"
        };

    public ShipmentService(IShipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ShipmentResponse>> GetShipmentsAsync(
        string? search,
        string? status,
        Guid? originLocationId,
        Guid? destinationLocationId,
        Guid? materialRequestId,
        DateTimeOffset? shippedAtFrom,
        DateTimeOffset? shippedAtTo,
        CancellationToken cancellationToken = default)
    {
        if (shippedAtFrom.HasValue &&
            shippedAtTo.HasValue &&
            shippedAtFrom.Value > shippedAtTo.Value)
        {
            throw new ArgumentException(
                "Shipped date from cannot be later than shipped date to."
            );
        }

        var shipments = await _repository.GetShipmentsAsync(
            search,
            status,
            originLocationId,
            destinationLocationId,
            materialRequestId,
            shippedAtFrom,
            shippedAtTo,
            cancellationToken
        );

        return shipments.Select(MapToResponse).ToList();
    }

    public async Task<ShipmentResponse?> GetShipmentByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var shipment = await _repository.GetShipmentByIdAsync(id,cancellationToken);
        return shipment == null? null: MapToResponse(shipment);
    }

    public async Task<ShipmentResponse> CreateShipmentAsync(CreateShipmentRequest request,CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var shipmentNumber = request.ShipmentNumber?.Trim();

        if (string.IsNullOrWhiteSpace(shipmentNumber))
        {
            throw new ArgumentException(
                "Shipment number is required."
            );
        }

        if (await _repository.ShipmentNumberExistsAsync(shipmentNumber,null,cancellationToken))
        {
            throw new InvalidOperationException(
                $"Shipment number '{shipmentNumber}' already exists."
            );
        }

        if (request.OriginLocationId == request.DestinationLocationId)
        {
            throw new ArgumentException(
                "Origin and destination locations must be different."
            );
        }

        await ValidateLocationAsync(request.OriginLocationId,cancellationToken
        );

        await ValidateLocationAsync(request.DestinationLocationId,cancellationToken
        );

        if (request.MaterialRequestId.HasValue)
        {
            var materialRequestExists = await _repository.MaterialRequestExistsAsync(request.MaterialRequestId.Value,cancellationToken
                );

            if (!materialRequestExists)
            {
                throw new KeyNotFoundException(
                    "Material request was not found."
                );
            }
        }

        var trackingNumber = NormalizeOptional(request.TrackingNumber);

        if (trackingNumber != null &&
            await _repository.TrackingNumberExistsAsync(trackingNumber,null,cancellationToken))
        {
            throw new InvalidOperationException(
                $"Tracking number '{trackingNumber}' already exists."
            );
        }

        var shippedAt = ToUtc(request.ShippedAt);
        var estimatedArrivalAt = ToUtc(request.EstimatedArrivalAt);

        ValidateShipmentDates(shippedAt, estimatedArrivalAt);

        var shipment = new Shipment
        {
            ShipmentNumber = shipmentNumber,
            MaterialRequestId = request.MaterialRequestId,
            OriginLocationId = request.OriginLocationId,
            DestinationLocationId = request.DestinationLocationId,
            TrackingNumber = trackingNumber,
            CarrierName = NormalizeOptional(request.CarrierName),
            ShippedAt = shippedAt,
            EstimatedArrivalAt = estimatedArrivalAt,
            Status = "PREPARING",
            Notes = NormalizeOptional(request.Notes)
        };

        await _repository.AddShipmentAsync(shipment,cancellationToken
        );

        await _repository.AddTrackingEventAsync(
            new ShipmentTrackingEvent
            {
                Shipment = shipment,
                Status = "PREPARING",
                Description = "Shipment created and is being prepared.",
                EventDate = DateTimeOffset.UtcNow
            },
            cancellationToken
        );

        await _repository.SaveChangesAsync(cancellationToken);

        var createdShipment = await _repository.GetShipmentByIdAsync(shipment.Id, cancellationToken
        );

        return createdShipment == null ? throw new InvalidOperationException(
                "Shipment was created but could not be retrieved."
            )
            : MapToResponse(createdShipment);
    }

    public async Task<ShipmentResponse> UpdateShipmentAsync(Guid id,UpdateShipmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var shipment = await _repository.GetShipmentForUpdateAsync(
            id,cancellationToken
        );

        if (shipment == null)
        {
            throw new KeyNotFoundException(
                $"Shipment with ID '{id}' was not found."
            );
        }

        if (shipment.Status is "DELIVERED" or "CANCELLED")
        {
            throw new InvalidOperationException(
                "A delivered or cancelled shipment cannot be updated."
            );
        }

        if (request.OriginLocationId == request.DestinationLocationId)
        {
            throw new ArgumentException(
                "Origin and destination locations must be different."
            );
        }

        await ValidateLocationAsync(request.OriginLocationId,cancellationToken
        );

        await ValidateLocationAsync(request.DestinationLocationId,cancellationToken
        );

        var trackingNumber = NormalizeOptional(request.TrackingNumber);

        if (trackingNumber != null &&
            await _repository.TrackingNumberExistsAsync(trackingNumber,id,cancellationToken))
        {
            throw new InvalidOperationException(
                $"Tracking number '{trackingNumber}' already exists."
            );
        }

        var shippedAt = ToUtc(request.ShippedAt);
        var estimatedArrivalAt = ToUtc(request.EstimatedArrivalAt);

        ValidateShipmentDates(shippedAt, estimatedArrivalAt);

        shipment.OriginLocationId = request.OriginLocationId;
        shipment.DestinationLocationId = request.DestinationLocationId;
        shipment.TrackingNumber = trackingNumber;
        shipment.CarrierName = NormalizeOptional(request.CarrierName);
        shipment.ShippedAt = shippedAt;
        shipment.EstimatedArrivalAt = estimatedArrivalAt;
        shipment.Notes = NormalizeOptional(request.Notes);

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedShipment = await _repository.GetShipmentByIdAsync(
            id,
            cancellationToken
        );

        return updatedShipment == null
            ? throw new InvalidOperationException(
                "Shipment was updated but could not be retrieved."
            )
            : MapToResponse(updatedShipment);
    }

    public async Task<ShipmentResponse> UpdateShipmentStatusAsync(Guid id,
        UpdateShipmentStatusRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var shipment = await _repository.GetShipmentForUpdateAsync(
            id,
            cancellationToken
        );

        if (shipment == null)
        {
            throw new KeyNotFoundException(
                $"Shipment with ID '{id}' was not found."
            );
        }

        var newStatus = request.Status?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(newStatus) ||
            !AllowedStatuses.Contains(newStatus))
        {
            throw new ArgumentException(
                "Invalid shipment status."
            );
        }

        ValidateStatusTransition(shipment.Status, newStatus);

        ValidateCoordinates(request.Latitude, request.Longitude);

        var eventDate = request.EventDate?.ToUniversalTime()
            ?? DateTimeOffset.UtcNow;

        if (shipment.ShippedAt.HasValue &&
            eventDate < shipment.ShippedAt.Value)
        {
            throw new ArgumentException(
                "Tracking event date cannot be earlier than shipped date."
            );
        }

        if (newStatus == "DELIVERED")
        {
            if (!shipment.ShippedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Shipment must have a shipped date before it can be delivered."
                );
            }

            shipment.DeliveredAt = eventDate;
        }

        if (newStatus == "IN_TRANSIT" &&
            !shipment.ShippedAt.HasValue)
        {
            shipment.ShippedAt = eventDate;
        }

        shipment.Status = newStatus;

        await _repository.AddTrackingEventAsync(
            new ShipmentTrackingEvent
            {
                ShipmentId = shipment.Id,
                Status = newStatus,
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? $"Shipment status changed to {newStatus}."
                    : request.Description.Trim(),
                LocationName = NormalizeOptional(request.LocationName),
                EventDate = eventDate,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            },
            cancellationToken
        );

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedShipment = await _repository.GetShipmentByIdAsync(id,cancellationToken
        );

        return updatedShipment == null
            ? throw new InvalidOperationException(
                "Shipment status was updated but could not be retrieved."
            )
            : MapToResponse(updatedShipment);
    }

    public async Task<ShipmentItemResponse> AddShipmentItemAsync(Guid shipmentId,
        Guid itemId,decimal quantity,string? notes,CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Shipment item quantity must be greater than zero."
            );
        }

        var shipment = await _repository.GetShipmentForUpdateAsync(shipmentId,cancellationToken
        );

        if (shipment == null)
        {
            throw new KeyNotFoundException(
                $"Shipment with ID '{shipmentId}' was not found."
            );
        }

        if (shipment.Status is "DELIVERED" or "CANCELLED")
        {
            throw new InvalidOperationException(
                "Items cannot be added to a delivered or cancelled shipment."
            );
        }

        var itemExists = await _repository.ItemExistsAsync(itemId,cancellationToken
        );

        if (!itemExists)
        {
            throw new KeyNotFoundException(
                $"Item with ID '{itemId}' was not found."
            );
        }

        if (shipment.Items.Any(x => x.ItemId == itemId))
        {
            throw new InvalidOperationException(
                "This item already exists in the shipment."
            );
        }

        var shipmentItem = new ShipmentItem
        {
            ShipmentId = shipmentId,
            ItemId = itemId,
            Quantity = quantity,
            Notes = NormalizeOptional(notes)
        };

        await _repository.AddShipmentItemAsync(shipmentItem,cancellationToken
        );

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedShipment = await _repository.GetShipmentByIdAsync(shipmentId,cancellationToken
        );

        var addedItem = updatedShipment?.Items
            .FirstOrDefault(x => x.Id == shipmentItem.Id);

        return addedItem == null
            ? throw new InvalidOperationException(
                "Shipment item was added but could not be retrieved."
            )
            : MapItemToResponse(addedItem);
    }

    private async Task ValidateLocationAsync(Guid locationId,CancellationToken cancellationToken)
    {
        var isActive = await _repository.IsLocationActiveAsync(locationId,cancellationToken
        );

        if (!isActive)
        {
            throw new ArgumentException(
                $"Location '{locationId}' does not exist or is inactive."
            );
        }
    }

    private static void ValidateShipmentDates(DateTimeOffset? shippedAt,DateTimeOffset? estimatedArrivalAt)
    {
        if (shippedAt.HasValue &&
            estimatedArrivalAt.HasValue &&
            estimatedArrivalAt.Value < shippedAt.Value)
        {
            throw new ArgumentException(
                "Estimated arrival date cannot be earlier than shipped date."
            );
        }
    }

    private static void ValidateCoordinates(decimal? latitude,decimal? longitude)
    {
        if (latitude.HasValue &&
            (latitude.Value < -90 || latitude.Value > 90))
        {
            throw new ArgumentException(
                "Latitude must be between -90 and 90."
            );
        }

        if (longitude.HasValue &&
            (longitude.Value < -180 || longitude.Value > 180))
        {
            throw new ArgumentException(
                "Longitude must be between -180 and 180."
            );
        }

        if (latitude.HasValue != longitude.HasValue)
        {
            throw new ArgumentException(
                "Latitude and longitude must be provided together."
            );
        }
    }

    private static void ValidateStatusTransition(string currentStatus,string newStatus)
    {
        var allowedTransitions = currentStatus.ToUpperInvariant() switch
        {
            "PREPARING" => new[] { "IN_TRANSIT", "CANCELLED" },
            "IN_TRANSIT" => new[] { "DELAYED", "DELIVERED", "CANCELLED" },
            "DELAYED" => new[] { "IN_TRANSIT", "DELIVERED", "CANCELLED" },
            _ => Array.Empty<string>()
        };

        if (!allowedTransitions.Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Shipment status cannot change from '{currentStatus}' to '{newStatus}'."
            );
        }
    }

    private static DateTimeOffset? ToUtc(DateTimeOffset? value)
    {
        return value?.ToUniversalTime();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null: value.Trim();
    }

    private static ShipmentResponse MapToResponse(Shipment shipment)
    {
        return new ShipmentResponse
        {
            Id = shipment.Id,
            ShipmentNumber = shipment.ShipmentNumber,
            MaterialRequestId = shipment.MaterialRequestId,

            OriginLocationId = shipment.OriginLocationId,
            OriginLocationCode = shipment.OriginLocation.Code,
            OriginLocationName = shipment.OriginLocation.Name,

            DestinationLocationId = shipment.DestinationLocationId,
            DestinationLocationCode = shipment.DestinationLocation.Code,
            DestinationLocationName = shipment.DestinationLocation.Name,

            TrackingNumber = shipment.TrackingNumber,
            CarrierName = shipment.CarrierName,
            ShippedAt = shipment.ShippedAt,
            EstimatedArrivalAt = shipment.EstimatedArrivalAt,
            DeliveredAt = shipment.DeliveredAt,
            Status = shipment.Status,
            Notes = shipment.Notes,
            CreatedByUserId = shipment.CreatedByUserId,

            Items = shipment.Items
                .Select(MapItemToResponse)
                .ToList(),

            TrackingEvents = shipment.TrackingEvents
                .OrderByDescending(x => x.EventDate)
                .Select(MapTrackingEventToResponse)
                .ToList(),

            CreatedAt = shipment.CreatedAt,
            UpdatedAt = shipment.UpdatedAt
        };
    }

    private static ShipmentItemResponse MapItemToResponse( ShipmentItem item)
    {
        return new ShipmentItemResponse
        {
            Id = item.Id,
            ItemId = item.ItemId,
            ItemCode = item.Item.Code,
            ItemName = item.Item.Name,
            Quantity = item.Quantity,
            Notes = item.Notes,
            CreatedAt = item.CreatedAt
        };
    }

    private static ShipmentTrackingEventResponse MapTrackingEventToResponse(ShipmentTrackingEvent trackingEvent)
    {
        return new ShipmentTrackingEventResponse
        {
            Id = trackingEvent.Id,
            Status = trackingEvent.Status,
            Description = trackingEvent.Description,
            LocationName = trackingEvent.LocationName,
            EventDate = trackingEvent.EventDate,
            Latitude = trackingEvent.Latitude,
            Longitude = trackingEvent.Longitude,
            CreatedAt = trackingEvent.CreatedAt
        };
    }
}
