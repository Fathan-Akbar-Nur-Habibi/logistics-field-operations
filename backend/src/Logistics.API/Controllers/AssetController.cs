using Logistics.Application.DTOs.Asset;
using Logistics.Application.Interfaces.Asset;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/assets")]
public class AssetController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAssets([FromQuery] string? search,[FromQuery] Guid? assetCategoryId,
        [FromQuery] Guid? locationId,[FromQuery] string? status,CancellationToken cancellationToken)
    {
        try
        {
            var result = await _assetService.GetAssetsAsync(
                search,
                assetCategoryId,
                locationId,
                status,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}", Name = nameof(GetAssetById))]
    public async Task<IActionResult> GetAssetById(Guid id,CancellationToken cancellationToken)
    {
        var result = await _assetService.GetAssetByIdAsync(
            id,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new { message = "Asset tidak ditemukan." });
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsset([FromBody] CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _assetService.CreateAssetAsync(
                request,
                cancellationToken);

            return CreatedAtRoute(
                nameof(GetAssetById),
                new { id = result.Id },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsset(Guid id,[FromBody] UpdateAssetRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _assetService.UpdateAssetAsync(
                id,
                request,
                cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("movements")]
    public async Task<IActionResult> GetAssetMovements([FromQuery] Guid? assetId,CancellationToken cancellationToken)
    {
        var result = await _assetService.GetAssetMovementsAsync(
            assetId,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/movements")]
    public async Task<IActionResult> MoveAsset(Guid id,[FromBody] AssetMovementRequest request,CancellationToken cancellationToken)
    {
        try
        {
            var result = await _assetService.MoveAssetAsync(
                id,
                request,
                cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
