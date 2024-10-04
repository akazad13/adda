using System.Threading.Tasks;
using AutoMapper;
using EasyConnect.API.Dtos;
using EasyConnect.API.Models;
using EasyConnect.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyConnect.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(
    IMapper mapper,
    UserManager<User> userManager,
    IAuthService authService
    ) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
    {
        var userToCreate = _mapper.Map<User>(userForRegisterDto);

        var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

        var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

        if (result.Succeeded)
        {
            return CreatedAtRoute(
                "GetUser",
                new { Controller = "Users", id = userToCreate.Id },
                userToReturn
            ); // temp
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
        var result = await _authService.LoginAsync(userForLoginDto);

        if(!result.IsError)
            return Ok(result.Value);

        return Unauthorized();
    }
}
