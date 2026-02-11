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
        var (ok, error) = await auth.RegisterAsync(request.Email, request.Password, cancellationToken);
        return ok ? NoContent() : BadRequest(new { error });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var (ok, token, error) = await auth.LoginAsync(request.Email, request.Password, cancellationToken);
        return ok ? Ok(new { token }) : Unauthorized(new { error });
    }
}