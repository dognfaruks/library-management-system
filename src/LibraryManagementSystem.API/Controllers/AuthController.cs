using LibraryManagementSystem.Application.DTOs.Auth;
using LibraryManagementSystem.Application.Exceptions;
using LibraryManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (DuplicateEmailException ex)
        {
            return Conflict(new { message = ex.Message }); // 409
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message }); // 401
        }
    }
        [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult Me()
    {
        var username = User.Identity?.Name;
        var roles = User.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value);

        return Ok(new { username, roles });
    }

    [HttpGet("admin-only")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "ADMIN")]
    public IActionResult AdminOnly()
    {
        return Ok(new { message = "Bu içeriği sadece ADMIN görebilir." });
    }
}