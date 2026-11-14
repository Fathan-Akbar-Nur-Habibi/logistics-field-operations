using Logistics.Application.DTOs.Maintenance.Plan;
using Logistics.Application.DTOs.Maintenance.WorkOrder;
using Logistics.Application.DTOs.Maintenance.Record;
using Logistics.Application.Interfaces.Maintenance;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/maintenance")]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;
    private readonly ILogger<MaintenanceController> _logger;
    public MaintenanceController(IMaintenanceService maintenanceService, ILogger<MaintenanceController> logger)
    {
        _maintenanceService = maintenanceService;
        _logger = logger;
    }

    // MAINTENANCE PLAN
    [HttpGet("plans")]
    public async Task<IActionResult> GetMaintenancePlans(
        [FromQuery] string? search,
        [FromQuery] Guid? assetId,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.GetMaintenancePlansAsync(
            search,
            assetId,
            isActive,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("plans/{id:guid}")]
    public async Task<IActionResult> GetMaintenancePlanById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.GetMaintenancePlanByIdAsync(
            id,
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Maintenance Plan was not found."
            });
        }

        return Ok(result);
    }

    [HttpPost("plans")]
    public async Task<IActionResult> CreateMaintenancePlan([FromBody] CreateMaintenancePlanRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _maintenanceService.CreateMaintenancePlanAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetMaintenancePlanById),
                new { id = result.Id },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("plans/{id:guid}")]
    public async Task<IActionResult> UpdateMaintenancePlan(Guid id, [FromBody] UpdateMaintenancePlanRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _maintenanceService.UpdateMaintenancePlanAsync(
                id,
                request,
                cancellationToken);

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
    }

    // MAINTENANCE WORK ORDER
    [HttpGet("work-orders")]
    public async Task<IActionResult> GetMaintenanceWorkOrders(
        [FromQuery] string? search,
        [FromQuery] Guid? assetId,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? maintenanceType,
        CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.GetMaintenanceWorkOrdersAsync(
            search,
            assetId,
            status,
            priority,
            maintenanceType,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("work-orders/{id:guid}")]
    public async Task<IActionResult> GetMaintenanceWorkOrderById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.GetMaintenanceWorkOrderByIdAsync(id, cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Maintenance Work Order was not found."
            });
        }

        return Ok(result);
    }

    [HttpPost("work-orders")]
    public async Task<IActionResult> CreateMaintenanceWorkOrder([FromBody] CreateMaintenanceWorkOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _maintenanceService.CreateMaintenanceWorkOrderAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetMaintenanceWorkOrderById), new { id = result.Id },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("work-orders/{id:guid}")]
    public async Task<IActionResult> UpdateMaintenanceWorkOrder(Guid id, [FromBody] UpdateMaintenanceWorkOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _maintenanceService.UpdateMaintenanceWorkOrderAsync(
                id,
                request,
                cancellationToken);

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

    [HttpPatch("work-orders/{id:guid}/status")]
    public async Task<IActionResult> UpdateMaintenanceWorkOrderStatus(Guid id, [FromBody] UpdateMaintenanceWorkOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _maintenanceService.UpdateMaintenanceWorkOrderStatusAsync(id, request, cancellationToken);
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

    // MAINTENANCE RECORD
    [HttpGet("records")]
    public async Task<IActionResult> GetMaintenanceRecords(
        [FromQuery] Guid? maintenanceWorkOrderId,
        CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.GetMaintenanceRecordsAsync(
            maintenanceWorkOrderId,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("records/{id:guid}")]
    public async Task<IActionResult> GetMaintenanceRecordById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _maintenanceService.GetMaintenanceRecordByIdAsync(id, cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Maintenance Record was not found."
            });
        }

        return Ok(result);
    }

    [HttpPost("records")]
    public async Task<IActionResult> CreateMaintenanceRecord([FromBody] CreateMaintenanceRecordRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _maintenanceService.CreateMaintenanceRecordAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetMaintenanceRecordById), new { id = result.Id }, result);
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
            _logger.LogError(
                ex,
                "Invalid operation while creating Maintenance Record.");

            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while creating Maintenance Record.");

            return StatusCode(500, new
            {
                message = "An unexpected error occurred while creating Maintenance Record."
            });
        }
    }
}
