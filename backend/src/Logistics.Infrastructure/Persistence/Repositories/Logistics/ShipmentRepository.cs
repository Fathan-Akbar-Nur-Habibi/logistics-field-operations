using Logistics.Application.Interfaces.Logistics;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Repositories.Logistics;

public class ShipmentRepository : IShipmentRepository
{
    private readonly LogisticsDbContext _context;

    public ShipmentRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Shipment>> GetShipmentsAsync(
        string? search,
        string? status,
        Guid? originLocationId,
        Guid? destinationLocationId,
        Guid? materialRequestId,
        DateTimeOffset? shippedAtFrom,
        DateTimeOffset? shippedAtTo,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Shipments
            .AsNoTracking()
            .Include(x => x.OriginLocation)
            .Include(x => x.DestinationLocation)
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .Include(x => x.TrackingEvents)
            .AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            var searchPattern = $"%{keyword}%";

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.ShipmentNumber,
                    searchPattern
                ) ||

                (x.TrackingNumber != null &&
                    EF.Functions.ILike(
                        x.TrackingNumber,
                        searchPattern
                    )) ||

                (x.CarrierName != null &&
                    EF.Functions.ILike(
                        x.CarrierName,
                        searchPattern
                    )) ||

                EF.Functions.ILike(
                    x.OriginLocation.Name,
                    searchPattern
                ) ||

                EF.Functions.ILike(x.DestinationLocation.Name,searchPattern
                ) ||

                // Search by item name
                x.Items.Any(si =>EF.Functions.ILike(si.Item.Name,searchPattern
                    )
                ) ||

                // Search by item code
                x.Items.Any(si =>EF.Functions.ILike(si.Item.Code,searchPattern
                    )
                )
            );
        }

        // Filter by status
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status.Trim());
        }

        // Filter by origin location
        if (originLocationId.HasValue)
        {
            query = query.Where(
                x => x.OriginLocationId == originLocationId.Value
            );
        }

        // Filter by destination location
        if (destinationLocationId.HasValue)
        {
            query = query.Where(
                x => x.DestinationLocationId == destinationLocationId.Value
            );
        }

        // Filter by material request
        if (materialRequestId.HasValue)
        {
            query = query.Where(
                x => x.MaterialRequestId == materialRequestId.Value
            );
        }

        // Filter by shipped date range
        if (shippedAtFrom.HasValue)
        {
            var fromUtc = shippedAtFrom.Value.ToUniversalTime();

            query = query.Where(
                x => x.ShippedAt >= fromUtc
            );
        }

        if (shippedAtTo.HasValue)
        {
            var toUtc = shippedAtTo.Value.ToUniversalTime();

            query = query.Where(
                x => x.ShippedAt <= toUtc
            );
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Shipment?> GetShipmentByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .AsNoTracking()
            .Include(x => x.OriginLocation)
            .Include(x => x.DestinationLocation)
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .Include(x => x.TrackingEvents)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken
            );
    }

    public async Task<Shipment?> GetShipmentForUpdateAsync(Guid id,CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.OriginLocation)
            .Include(x => x.DestinationLocation)
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .Include(x => x.TrackingEvents)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken
            );
    }

    public async Task<bool> ShipmentNumberExistsAsync(string shipmentNumber,Guid? excludeShipmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Shipments
            .AsNoTracking()
            .Where(x => x.ShipmentNumber == shipmentNumber);

        if (excludeShipmentId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeShipmentId.Value
            );
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> TrackingNumberExistsAsync(string trackingNumber,Guid? excludeShipmentId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Shipments
            .AsNoTracking()
            .Where(x => x.TrackingNumber == trackingNumber);

        if (excludeShipmentId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeShipmentId.Value
            );
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsLocationActiveAsync(Guid locationId,CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == locationId && x.IsActive,
                cancellationToken
            );
    }

    public async Task<bool> MaterialRequestExistsAsync(Guid materialRequestId,
        CancellationToken cancellationToken = default)
    {
        return await _context.MaterialRequests
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == materialRequestId,
                cancellationToken
            );
    }

    public async Task<bool> ItemExistsAsync(Guid itemId,CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == itemId,
                cancellationToken
            );
    }

    public async Task AddShipmentAsync(Shipment shipment,CancellationToken cancellationToken = default)
    {
        await _context.Shipments.AddAsync(shipment,cancellationToken
        );
    }

    public async Task AddShipmentItemAsync(ShipmentItem shipmentItem,CancellationToken cancellationToken = default)
    {
        await _context.ShipmentItems.AddAsync(shipmentItem,cancellationToken
        );
    }

    public async Task AddTrackingEventAsync(ShipmentTrackingEvent trackingEvent,
        CancellationToken cancellationToken = default)
    {
        await _context.ShipmentTrackingEvents.AddAsync(trackingEvent,cancellationToken
        );
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
