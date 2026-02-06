using Microsoft.AspNetCore.Mvc;

namespace SimpleECommerceBackend.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/health-check")]
[ApiVersion("1.0")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Healthy()
    {
        return Ok("Healthy");
    }
}