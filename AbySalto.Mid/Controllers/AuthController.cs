using AbySalto.Mid.Application.Requests.Auth;
using AbySalto.Mid.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abysalto.Mid.Controller;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        await auth.RegisterAsync(request.Email, request.Password, cancellationToken);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var token = await auth.LoginAsync(request.Email, request.Password, cancellationToken);
        return Ok(new { token });
    }
}