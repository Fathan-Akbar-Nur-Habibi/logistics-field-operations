
using Logistics.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/database")]
public class DatabaseHealthController : ControllerBase
{
    private readonly LogisticsDbContext _context;

    public DatabaseHealthController(LogisticsDbContext context)
    {
        _context = context;
    }

    [HttpGet("health")]
    public async Task<IActionResult> CheckConnection()
    {
        try
        {
            bool connected = await _context.Database.CanConnectAsync();

            if (!connected)
            {
                return StatusCode(503, new
                {
                    isSuccess = false,
                    message = "Database connection failed"
                });
            }

            return Ok(new
            {
                isSuccess = true,
                message = "Successfully connected to Supabase PostgreSQL"
            });
        }
        catch
        {
            return StatusCode(503, new
            {
                isSuccess = false,
                message = "Unable to connect to database"
            });
        }
    }
}
