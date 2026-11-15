using Logistics.Application.DTOs.Crew.FieldAssignment;
using Logistics.Application.Interfaces.Crew;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/crew/field-assignments")]
public class FieldAssignmentController : ControllerBase
{
    private readonly IFieldAssignmentService _service;
    private readonly ILogger<FieldAssignmentController> _logger;

    public FieldAssignmentController(IFieldAssignmentService service, ILogger<FieldAssignmentController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET: api/crew/field-assignments
    [HttpGet]
    public async Task<IActionResult> GetFieldAssignments(
        [FromQuery] string? search,
        [FromQuery] Guid? locationId,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] DateTimeOffset? startDateFrom,
        [FromQuery] DateTimeOffset? startDateTo,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetFieldAssignmentsAsync(
                search,
                locationId,
                status,
                priority,
                startDateFrom,
                startDateTo,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving field assignments.");

            return StatusCode(500, new
            {
                message = "An error occurred while retrieving field assignments."
            });
        }
    }

    // GET: api/crew/field-assignments/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFieldAssignmentById(Guid id,CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetFieldAssignmentByIdAsync(id,cancellationToken);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Field assignment not found."
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
            _logger.LogError(ex,"Error retrieving field assignment {Id}.",id);

            return StatusCode(500, new
            {
                message = "An error occurred while retrieving field assignment."
            });
        }
    }

    // POST: api/crew/field-assignments
    [HttpPost]
    public async Task<IActionResult> CreateFieldAssignment([FromBody] CreateFieldAssignmentRequest request,CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CreateFieldAssignmentAsync(request,cancellationToken);

            return CreatedAtAction(nameof(GetFieldAssignmentById),new { id = result.Id },result);
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
            _logger.LogError(ex, "Error creating field assignment.");

            return StatusCode(500, new
            {
                message = "An error occurred while creating field assignment."
            });
        }
    }

    // PUT: api/crew/field-assignments/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateFieldAssignment(Guid id,[FromBody] UpdateFieldAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateFieldAssignmentAsync(
                id,
                request,
                cancellationToken);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Field assignment not found."
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
            _logger.LogError(ex,"Error updating field assignment {Id}.",id);

            return StatusCode(500, new
            {
                message = "An error occurred while updating field assignment."
            });
        }
    }

    // PATCH: api/crew/field-assignments/{id}/status
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateFieldAssignmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateStatusAsync(
                id,
                request.Status,
                cancellationToken);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Field assignment not found."
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
            _logger.LogError(
                ex,
                "Error updating field assignment status {Id}.",
                id);

            return StatusCode(500, new
            {
                message = "An error occurred while updating assignment status."
            });
        }
    }

    // POST: api/crew/field-assignments/{id}/members
    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(
        Guid id,
        [FromBody] AddFieldAssignmentMemberRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.AddMemberAsync(
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error adding member to field assignment {Id}.",
                id);

            return StatusCode(500, new
            {
                message = "An error occurred while adding assignment member."
            });
        }
    }

    // POST: api/crew/field-assignments/{id}/members/{memberId}/check-in
    [HttpPost("{id:guid}/members/{memberId:guid}/check-in")]
    public async Task<IActionResult> CheckInMember(
        Guid id,
        Guid memberId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CheckInMemberAsync(
                id,
                memberId,
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error checking in member {MemberId} for assignment {Id}.",
                memberId,
                id);

            return StatusCode(500, new
            {
                message = "An error occurred while checking in assignment member."
            });
        }
    }

    // POST: api/crew/field-assignments/{id}/members/{memberId}/check-out
    [HttpPost("{id:guid}/members/{memberId:guid}/check-out")]
    public async Task<IActionResult> CheckOutMember(
        Guid id,
        Guid memberId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.CheckOutMemberAsync(
                id,
                memberId,
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error checking out member {MemberId} for assignment {Id}.",
                memberId,
                id);

            return StatusCode(500, new
            {
                message = "An error occurred while checking out assignment member."
            });
        }
    }
}
