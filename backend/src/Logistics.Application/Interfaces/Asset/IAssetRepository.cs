using Logistics.Domain.Entities;
namespace Logistics.Application.Interfaces.Asset;

public interface IAssetRepository
{
    Task<List<global::Logistics.Domain.Entities.Asset>> GetAssetsAsync(
        string? search = null,
        Guid? assetCategoryId = null,
        Guid? locationId = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<global::Logistics.Domain.Entities.Asset?> GetAssetByIdAsync(Guid id,CancellationToken cancellationToken = default);
    Task<bool> AssetCodeExistsAsync(string assetCode,Guid? excludeId = null,CancellationToken cancellationToken = default);
    Task<bool> QRCodeExistsAsync(string qrCode,Guid? excludeId = null,CancellationToken cancellationToken = default);
    Task<bool> AssetCategoryExistsAsync(Guid assetCategoryId,CancellationToken cancellationToken = default);
    Task<bool> LocationExistsAsync(Guid locationId,CancellationToken cancellationToken = default);
    Task<bool> AssetCategoryIsActiveAsync(Guid assetCategoryId,CancellationToken cancellationToken = default);
    Task<bool> LocationIsActiveAsync(Guid locationId,CancellationToken cancellationToken = default);
    Task AddAssetAsync(global::Logistics.Domain.Entities.Asset asset,CancellationToken cancellationToken = default);
    Task<List<AssetMovement>> GetAssetMovementsAsync(Guid? assetId = null,CancellationToken cancellationToken = default);
    Task AddAssetMovementAsync(AssetMovement movement,CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation,CancellationToken cancellationToken = default);
    Task<global::Logistics.Domain.Entities.Asset?> LockAssetForUpdateAsync(Guid assetId,CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<global::Logistics.Domain.Entities.Asset?> GetAssetForUpdateAsync(Guid id,CancellationToken cancellationToken = default);
}
