using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.DTOs;
using StudyHub.Core.Services;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController:ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var res = await _authService.LoginAsync(request);

        return res == null ? Unauthorized() : Ok(res);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var res = await _authService.RegisterAsync(request);

        return Ok(res);
    }
    
    [HttpGet("debug-claims")]
    [Authorize]
    public IActionResult DebugClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }

}