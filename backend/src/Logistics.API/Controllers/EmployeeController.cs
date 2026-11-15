using Logistics.Application.DTOs.Crew;
using Logistics.Application.Interfaces.Crew;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/crew/employees")]
public class EmployeeController : ControllerBase
{
    private readonly ICrewService _crewService;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(ICrewService crewService,ILogger<EmployeeController> logger)
    {
        _crewService = crewService;
        _logger = logger;
    }

    // GET: api/crew/employees
    [HttpGet]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] string? search,
        [FromQuery] string? department,
        [FromQuery] string? position,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await _crewService.GetEmployeesAsync(search,department,position,isActive,cancellationToken);

        return Ok(result);
    }

    // GET: api/crew/employees/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEmployeeById(Guid id,CancellationToken cancellationToken)
    {
        var result = await _crewService.GetEmployeeByIdAsync(id,cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Employee was not found."
            });
        }

        return Ok(result);
    }

    // POST: api/crew/employees
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request,CancellationToken cancellationToken)
    {
        try
        {
            var result = await _crewService.CreateEmployeeAsync(request,cancellationToken);

            return CreatedAtAction(nameof(GetEmployeeById),new { id = result.Id },result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Unexpected error while creating Employee.");

            return StatusCode(500, new
            {
                message = "An unexpected error occurred while creating Employee."
            });
        }
    }

    // PUT: api/crew/employees/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEmployee(Guid id,[FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _crewService.UpdateEmployeeAsync(id,request,cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Unexpected error while updating Employee.");

            return StatusCode(500, new
            {
                message = "An unexpected error occurred while updating Employee."
            });
        }
    }
}
