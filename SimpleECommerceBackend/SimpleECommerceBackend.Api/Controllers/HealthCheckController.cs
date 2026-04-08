using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace SimpleECommerceBackend.Api.Controllers;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/health-check")]
[ApiVersion("1.0")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Healthy()
    {
        return Ok("Healthy");
    }
}