using Logistics.Application.DTOs.Logistics.Shipment;
using Logistics.Application.Interfaces.Logistics;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers.Logistics;

[ApiController]
[Route("api/logistics/shipments")]
public class ShipmentController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    // GET: api/logistics/shipments
    [HttpGet]
    [ProducesResponseType(typeof(List<ShipmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShipments(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? originLocationId,
        [FromQuery] Guid? destinationLocationId,
        [FromQuery] Guid? materialRequestId,
        [FromQuery] DateTimeOffset? shippedAtFrom,
        [FromQuery] DateTimeOffset? shippedAtTo,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shipmentService.GetShipmentsAsync(
                search,
                status,
                originLocationId,
                destinationLocationId,
                materialRequestId,
                shippedAtFrom,
                shippedAtTo,
                cancellationToken
            );

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/logistics/shipments/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShipmentById(Guid id,CancellationToken cancellationToken)
    {
        var result = await _shipmentService.GetShipmentByIdAsync(
            id,
            cancellationToken
        );

        if (result == null)
        {
            return NotFound(new
            {
                message = $"Shipment with ID '{id}' was not found."
            });
        }

        return Ok(result);
    }

    // POST: api/logistics/shipments
    [HttpPost]
    [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request,CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shipmentService.CreateShipmentAsync(request,cancellationToken
            );

            return CreatedAtAction(nameof(GetShipmentById),new { id = result.Id },result
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/logistics/shipments/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateShipment(Guid id,[FromBody] UpdateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shipmentService.UpdateShipmentAsync(id,request,cancellationToken
            );

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PATCH: api/logistics/shipments/{id}/status
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    
    public async Task<IActionResult> UpdateShipmentStatus(Guid id,[FromBody] UpdateShipmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shipmentService.UpdateShipmentStatusAsync(id,request,cancellationToken
            );

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // POST: api/logistics/shipments/{id}/items
    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(ShipmentItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddShipmentItem(
        Guid id,
        [FromBody] AddShipmentItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shipmentService.AddShipmentItemAsync(
                id,
                request.ItemId,
                request.Quantity,
                request.Notes,
                cancellationToken
            );

            return StatusCode(
                StatusCodes.Status201Created,
                result
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
