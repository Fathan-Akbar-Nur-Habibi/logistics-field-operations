using Logistics.Application.Interfaces.Asset;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Persistence.Repositories.Asset;

public class AssetRepository : IAssetRepository
{
    private readonly LogisticsDbContext _context;

    public AssetRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<List<global::Logistics.Domain.Entities.Asset>> GetAssetsAsync(
        string? search = null,
        Guid? assetCategoryId = null,
        Guid? locationId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Assets
            .AsNoTracking()
            .Include(x => x.AssetCategory)
            .Include(x => x.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.AssetCode, $"%{keyword}%") ||
                EF.Functions.ILike(x.Name, $"%{keyword}%") ||
                (x.SerialNumber != null &&
                 EF.Functions.ILike(x.SerialNumber, $"%{keyword}%")) ||
                (x.Brand != null &&
                 EF.Functions.ILike(x.Brand, $"%{keyword}%")));
        }

        if (assetCategoryId.HasValue)
        {
            query = query.Where(x =>
                x.AssetCategoryId == assetCategoryId.Value);
        }

        if (locationId.HasValue)
        {
            query = query.Where(x =>
                x.LocationId == locationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();

            query = query.Where(x =>
                x.Status == normalizedStatus);
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<global::Logistics.Domain.Entities.Asset?> GetAssetByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AsNoTracking()
            .Include(x => x.AssetCategory)
            .Include(x => x.Location)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> AssetCodeExistsAsync(string assetCode, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Assets
            .AsNoTracking()
            .Where(x => x.AssetCode == assetCode);

        if (excludeId.HasValue)
        {
            query = query.Where(x =>
                x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> QRCodeExistsAsync(string qrCode, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Assets
            .AsNoTracking()
            .Where(x => x.QRCode == qrCode);

        if (excludeId.HasValue)
        {
            query = query.Where(x =>
                x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> AssetCategoryExistsAsync(Guid assetCategoryId, CancellationToken cancellationToken = default)
    {
        return await _context.AssetCategories
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == assetCategoryId,
                cancellationToken);
    }

    public async Task<bool> AssetCategoryIsActiveAsync(Guid assetCategoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetCategories
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == assetCategoryId && x.IsActive,
                cancellationToken);
    }

    public async Task<bool> LocationExistsAsync(Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == locationId,
                cancellationToken);
    }

    public async Task<bool> LocationIsActiveAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == locationId && x.IsActive,
                cancellationToken);
    }

    public async Task AddAssetAsync(global::Logistics.Domain.Entities.Asset asset,
        CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(
            asset,
            cancellationToken);
    }

    public async Task<List<AssetMovement>> GetAssetMovementsAsync(Guid? assetId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AssetMovements
            .AsNoTracking()
            .Include(x => x.Asset)
            .Include(x => x.FromLocation)
            .Include(x => x.ToLocation)
            .AsQueryable();

        if (assetId.HasValue)
        {
            query = query.Where(x =>
                x.AssetId == assetId.Value);
        }

        return await query
            .OrderByDescending(x => x.MovementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAssetMovementAsync(AssetMovement movement, CancellationToken cancellationToken = default)
    {
        await _context.AssetMovements.AddAsync(
            movement,
            cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var result = await operation(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }

    public async Task<global::Logistics.Domain.Entities.Asset?> LockAssetForUpdateAsync(
        Guid assetId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .FromSqlInterpolated(
                $"SELECT * FROM assets WHERE \"Id\" = {assetId} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<global::Logistics.Domain.Entities.Asset?> GetAssetForUpdateAsync(
    Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .SingleOrDefaultAsync( x => x.Id == id, cancellationToken);
    }
}
