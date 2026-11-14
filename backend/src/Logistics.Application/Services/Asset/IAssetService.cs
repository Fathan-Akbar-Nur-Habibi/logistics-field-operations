using Logistics.Application.DTOs.Asset;

namespace Logistics.Application.Interfaces.Asset;

public interface IAssetService
{
    Task<List<AssetResponse>> GetAssetsAsync(string? search = null,Guid? assetCategoryId = null,
        Guid? locationId = null, string? status = null,CancellationToken cancellationToken = default);
    Task<AssetResponse?> GetAssetByIdAsync(Guid id,CancellationToken cancellationToken = default);
    Task<AssetResponse> CreateAssetAsync(CreateAssetRequest request,CancellationToken cancellationToken = default);
    Task<AssetResponse> UpdateAssetAsync(Guid id,UpdateAssetRequest request,CancellationToken cancellationToken = default);
    Task<List<AssetMovementHistoryResponse>> GetAssetMovementsAsync(Guid? assetId = null,CancellationToken cancellationToken = default);
    Task<AssetMovementResponse> MoveAssetAsync(Guid assetId,AssetMovementRequest request,CancellationToken cancellationToken = default);
}
