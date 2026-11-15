using Logistics.Application.DTOs.Crew.CrewSchedule;
using Logistics.Application.Interfaces.Crew;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/crew/schedules")]
public class CrewScheduleController : ControllerBase
{
    private readonly ICrewScheduleService _service;
    private readonly ILogger<CrewScheduleController> _logger;

    public CrewScheduleController(
        ICrewScheduleService service,
        ILogger<CrewScheduleController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCrewSchedules(
        [FromQuery] DateOnly? workDate,
        [FromQuery] Guid? employeeId,
        [FromQuery] Guid? shiftId,
        [FromQuery] Guid? locationId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetCrewSchedulesAsync(
                workDate,
                employeeId,
                shiftId,
                locationId,
                status,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving crew schedules.");
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving crew schedules."
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCrewScheduleById(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetCrewScheduleByIdAsync(
                id,
                cancellationToken);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Crew schedule not found."
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
            _logger.LogError(ex, "Error retrieving crew schedule {Id}.", id);
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving crew schedule."
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCrewSchedule(
        [FromBody] CreateCrewScheduleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateCrewScheduleAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetCrewScheduleById),
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating crew schedule.");
            return StatusCode(500, new
            {
                message = "An error occurred while creating crew schedule."
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCrewSchedule(Guid id,[FromBody] UpdateCrewScheduleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateCrewScheduleAsync(
                id,
                request,
                cancellationToken);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Crew schedule not found."
                });
            }

            return Ok(result);
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
            _logger.LogError(ex, "Error updating crew schedule {Id}.", id);
            return StatusCode(500, new
            {
                message = "An error occurred while updating crew schedule."
            });
        }
    }
}
