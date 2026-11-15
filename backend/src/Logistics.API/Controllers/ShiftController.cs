using Logistics.Application.DTOs.Crew.Shift;
using Logistics.Application.Interfaces.Crew;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/crew/shifts")]
public class ShiftController : ControllerBase
{
    private readonly IShiftService _service;
    private readonly ILogger<ShiftController> _logger;

    public ShiftController(IShiftService service,ILogger<ShiftController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET: api/crew/shifts
    [HttpGet]
    public async Task<IActionResult> GetShifts([FromQuery] string? search,[FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetShiftsAsync(search,isActive,
                cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shifts.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving shifts." });
        }
    }

    // GET: api/crew/shifts/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetShiftById(Guid id,CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetShiftByIdAsync(
                id,cancellationToken);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Data shift Not Found."
                });
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shift {ShiftId}.", id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving shift details." });
        }
    }

    // POST: api/crew/shifts
    [HttpPost]
    public async Task<IActionResult> CreateShift([FromBody] CreateShiftRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateShiftAsync(request,cancellationToken);

            return CreatedAtAction(nameof(GetShiftById),new { id = result.Id },result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating shift.");

            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while creating the shift." });
        }
    }

    // PUT: api/crew/shifts/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateShift(Guid id,[FromBody] UpdateShiftRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateShiftAsync(id,request,cancellationToken);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating shift {ShiftId}.", id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while updating the shift." });
        }
    }
}
