using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EasyConnect.API.Dtos;
using EasyConnect.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EasyConnect.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(
    IConfiguration config,
    IMapper mapper,
    UserManager<User> userManager,
    SignInManager<User> signInManager
    ) : ControllerBase
{
    private readonly IConfiguration _config = config;
    private readonly IMapper _mapper = mapper;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly UserManager<User> _userManager = userManager;

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
        var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(
                u =>
                    u.UserName.Equals(
                        userForLoginDto.Username
                    )
            );

        if (user == null)
        {
            return Unauthorized();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            userForLoginDto.Password,
            false
        );

        if (result.Succeeded)
        {
            var appUser = _mapper.Map<UserForNavbarDto>(user);

            return Ok(new { token = GenerateJwtToken(user).Result, user = appUser });
        }

        return Unauthorized();
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // After this, inject the IConfiguration class, then in appsetting add Token
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
