using Adda.API.Dtos;
using Adda.API.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adda.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(
    IAuthService authService
    ) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(AuthRequest request)
    {
        ErrorOr.ErrorOr<AuthResponse> result = await _authService.LoginAsync(request);

        if (!result.IsError)
        {
            return Ok(result.Value);
        }

        return Unauthorized();
    }
}
