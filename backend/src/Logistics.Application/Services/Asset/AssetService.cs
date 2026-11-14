using Logistics.Application.DTOs.Asset;
using Logistics.Application.Interfaces.Asset;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services.Asset;

using AssetEntity = global::Logistics.Domain.Entities.Asset;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _repository;

    private static readonly HashSet<string> AllowedStatuses =
    [
        "AVAILABLE",
        "IN_USE",
        "MAINTENANCE",
        "RETIRED"
    ];

    public AssetService(IAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AssetResponse>> GetAssetsAsync(string? search = null,Guid? assetCategoryId = null,
        Guid? locationId = null,string? status = null,CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(status))
        {
            status = NormalizeStatus(status);

            ValidateStatus(status);
        }

        var assets = await _repository.GetAssetsAsync(search,assetCategoryId,locationId,
            status,cancellationToken);

        return assets.Select(MapAssetToResponse).ToList();
    }

    public async Task<AssetResponse?> GetAssetByIdAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var asset = await _repository.GetAssetByIdAsync(id,cancellationToken);
        return asset == null? null: MapAssetToResponse(asset);
    }

    public async Task<AssetResponse> CreateAssetAsync(CreateAssetRequest request,CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.AssetCode))
        {
            throw new ArgumentException("Asset code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Asset name is required.");
        }

        if (request.AssetCategoryId == Guid.Empty)
        {
            throw new ArgumentException("Asset category is required.");
        }

        if (request.LocationId == Guid.Empty)
        {
            throw new ArgumentException("Asset location is required.");
        }

        if (request.PurchasePrice < 0)
        {
            throw new ArgumentException(
                "Purchase price cannot be negative.");
        }

        var assetCode = request.AssetCode.Trim().ToUpperInvariant();

        var name = request.Name.Trim();

        if (await _repository.AssetCodeExistsAsync(assetCode,cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"Asset code '{assetCode}' already exists.");
        }

        if (!await _repository.AssetCategoryIsActiveAsync(request.AssetCategoryId,
            cancellationToken))
        {
            throw new ArgumentException(
                "Asset category does not exist or is inactive.");
        }

        if (!await _repository.LocationIsActiveAsync(request.LocationId,
            cancellationToken))
        {
            throw new ArgumentException(
                "Location does not exist or is inactive.");
        }

        var qrCode = NormalizeOptional(request.QRCode);

        if (qrCode != null &&
            await _repository.QRCodeExistsAsync(qrCode,
                cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException(
                $"QR Code '{qrCode}' already exists.");
        }

        var asset = new AssetEntity
        {
            Id = Guid.NewGuid(),

            AssetCode = assetCode,
            Name = name,
            Description = NormalizeOptional(request.Description),

            AssetCategoryId = request.AssetCategoryId,
            LocationId = request.LocationId,

            SerialNumber = NormalizeOptional(request.SerialNumber),
            Brand = NormalizeOptional(request.Brand),
            Model = NormalizeOptional(request.Model),

            PurchaseDate = request.PurchaseDate,
            PurchasePrice = request.PurchasePrice,
            WarrantyExpiryDate = request.WarrantyExpiryDate,

            Status = "AVAILABLE",

            QRCode = qrCode,
            Notes = NormalizeOptional(request.Notes),

            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddAssetAsync(asset,
            cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        var createdAsset = await _repository.GetAssetByIdAsync(asset.Id,cancellationToken);

        if (createdAsset == null)
        {
            throw new InvalidOperationException(
                "Failed to retrieve the created asset.");
        }

        return MapAssetToResponse(createdAsset);
    }

    public async Task<AssetResponse> UpdateAssetAsync(Guid id,UpdateAssetRequest request,CancellationToken cancellationToken = default)
    {
        var asset = await _repository.GetAssetForUpdateAsync(
            id,
            cancellationToken);

        if (asset == null)
        {
            throw new KeyNotFoundException(
                $"Asset with ID '{id}' was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Asset name is required.");
        }

        if (request.AssetCategoryId == Guid.Empty)
        {
            throw new ArgumentException("Asset category is required.");
        }

        if (request.PurchasePrice < 0)
        {
            throw new ArgumentException("Purchase price cannot be negative.");
        }

        var status = NormalizeStatus(request.Status);
        ValidateStatus(status);

        if (!await _repository.AssetCategoryIsActiveAsync(request.AssetCategoryId,
            cancellationToken))
        {
            throw new ArgumentException("Asset category does not exist or is inactive.");
        }

        var qrCode = NormalizeOptional(request.QRCode);

        if (qrCode != null &&await _repository.QRCodeExistsAsync(qrCode,id,cancellationToken))
        {
            throw new InvalidOperationException(
                $"QR Code '{qrCode}' already belongs to another asset.");
        }

        asset.Name = request.Name.Trim();
        asset.Description = NormalizeOptional(request.Description);

        asset.AssetCategoryId = request.AssetCategoryId;

        asset.SerialNumber = NormalizeOptional(request.SerialNumber);
        asset.Brand = NormalizeOptional(request.Brand);
        asset.Model = NormalizeOptional(request.Model);

        asset.PurchaseDate = request.PurchaseDate;
        asset.PurchasePrice = request.PurchasePrice;
        asset.WarrantyExpiryDate = request.WarrantyExpiryDate;

        asset.Status = status;

        asset.QRCode = qrCode;
        asset.Notes = NormalizeOptional(request.Notes);

        asset.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        var updatedAsset = await _repository.GetAssetByIdAsync(
            id,
            cancellationToken);

        if (updatedAsset == null)
        {
            throw new InvalidOperationException(
                "Failed to retrieve the updated asset.");
        }

        return MapAssetToResponse(updatedAsset);
    }

    public async Task<List<AssetMovementHistoryResponse>>
        GetAssetMovementsAsync(Guid? assetId = null,CancellationToken cancellationToken = default)
    {
        var movements = await _repository.GetAssetMovementsAsync(
            assetId,
            cancellationToken);

        return movements.Select(x => new AssetMovementHistoryResponse
        {
            Id = x.Id,

            AssetId = x.AssetId,
            AssetCode = x.Asset.AssetCode,
            AssetName = x.Asset.Name,

            FromLocationName = x.FromLocation.Name,
            ToLocationName = x.ToLocation.Name,

            MovementDate = x.MovementDate,
            Reason = x.Reason

        }).ToList();
    }

    public async Task<AssetMovementResponse> MoveAssetAsync(Guid assetId,AssetMovementRequest request,
        CancellationToken cancellationToken = default)
    {
        if (assetId == Guid.Empty)
        {
            throw new ArgumentException("Asset ID is required.");
        }

        if (request.ToLocationId == Guid.Empty)
        {
            throw new ArgumentException("Destination location is required.");
        }
        var movementId = await _repository.ExecuteInTransactionAsync(
            async token =>
            {
                var asset = await _repository.LockAssetForUpdateAsync(assetId,token);
                if (asset == null)
                {
                    throw new KeyNotFoundException(
                        $"Asset with ID '{assetId}' was not found.");
                }

                if (asset.Status == "RETIRED")
                {
                    throw new InvalidOperationException("Retired assets cannot be moved.");
                }

                if (asset.LocationId == request.ToLocationId)
                {
                    throw new ArgumentException("Destination location must be different from current location.");
                }

                var isDestinationActive = await _repository.LocationIsActiveAsync(request.ToLocationId,token);
                if (!isDestinationActive)
                {
                    throw new ArgumentException("Destination location does not exist or is inactive.");
                }

                var fromLocationId = asset.LocationId;

                var movement = new AssetMovement
                {
                    Id = Guid.NewGuid(),

                    AssetId = asset.Id,

                    FromLocationId = fromLocationId,
                    ToLocationId = request.ToLocationId,

                    MovementDate = DateTimeOffset.UtcNow,

                    Reason = NormalizeOptional(request.Reason),

                    MovedByUserId = null
                };

                asset.LocationId = request.ToLocationId;
                asset.UpdatedAt = DateTimeOffset.UtcNow;
                await _repository.AddAssetMovementAsync(movement,token);
                await _repository.SaveChangesAsync(token);

                return movement.Id;
            },
            cancellationToken);

        var movements = await _repository.GetAssetMovementsAsync(assetId,cancellationToken);

        var savedMovement = movements.SingleOrDefault(x => x.Id == movementId);
        if (savedMovement == null)
        {
            throw new InvalidOperationException("Asset movement was saved but could not be retrieved.");
        }

        return MapMovementToResponse(savedMovement);
    }

    private static AssetResponse MapAssetToResponse(AssetEntity asset)
    {
        return new AssetResponse
        {
            Id = asset.Id,

            AssetCode = asset.AssetCode,
            Name = asset.Name,
            Description = asset.Description,

            AssetCategoryId = asset.AssetCategoryId,
            AssetCategoryCode = asset.AssetCategory.Code,
            AssetCategoryName = asset.AssetCategory.Name,

            LocationId = asset.LocationId,
            LocationCode = asset.Location.Code,
            LocationName = asset.Location.Name,

            SerialNumber = asset.SerialNumber,
            Brand = asset.Brand,
            Model = asset.Model,

            PurchaseDate = asset.PurchaseDate,
            PurchasePrice = asset.PurchasePrice,
            WarrantyExpiryDate = asset.WarrantyExpiryDate,

            Status = asset.Status,

            QRCode = asset.QRCode,
            Notes = asset.Notes,

            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt
        };
    }


    private static AssetMovementResponse MapMovementToResponse(
        AssetMovement movement)
    {
        return new AssetMovementResponse
        {
            Id = movement.Id,

            AssetId = movement.AssetId,
            AssetCode = movement.Asset.AssetCode,
            AssetName = movement.Asset.Name,

            FromLocationId = movement.FromLocationId,
            FromLocationCode = movement.FromLocation.Code,
            FromLocationName = movement.FromLocation.Name,

            ToLocationId = movement.ToLocationId,
            ToLocationCode = movement.ToLocation.Code,
            ToLocationName = movement.ToLocation.Name,

            MovementDate = movement.MovementDate,
            Reason = movement.Reason,

            MovedByUserId = movement.MovedByUserId
        };
    }

    private static string NormalizeStatus(string? status)
    {
        return status?.Trim().ToUpperInvariant() ?? string.Empty;
    }

    private static void ValidateStatus(string status)
    {
        if (!AllowedStatuses.Contains(status))
        {
            throw new ArgumentException(
                $"Invalid asset status '{status}'. " +
                "Allowed values: AVAILABLE, IN_USE, MAINTENANCE, RETIRED.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
