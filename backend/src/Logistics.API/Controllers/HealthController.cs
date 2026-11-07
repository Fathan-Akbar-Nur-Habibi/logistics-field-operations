
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            isSuccess = true,
            message = "Logistics API is running",
            version = "1.0.0",
            timestamp = DateTime.UtcNow
        });
    }
}
