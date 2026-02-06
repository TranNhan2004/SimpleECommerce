using Microsoft.AspNetCore.Mvc;

namespace SimpleECommerceBackend.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public class AuthenticationController
{
}