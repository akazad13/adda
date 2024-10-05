using System.Threading.Tasks;
using EasyConnect.API.Dtos;
using EasyConnect.API.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyConnect.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(
    IAuthService authService
    ) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
        ErrorOr.ErrorOr<AuthResponse> result = await _authService.LoginAsync(userForLoginDto);

        if (!result.IsError)
        {
            return Ok(result.Value);
        }

        return Unauthorized();
    }
}
