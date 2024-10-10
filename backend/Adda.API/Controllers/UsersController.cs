using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Security.CurrentUserProvider;
using Adda.API.Services.UserService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Adda.API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/users")]
public class UsersController(
    IMapper mapper,
    ICurrentUserProvider currentUser,
    IUserService userService
) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IUserService _userService = userService;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegistrationRequest request)
    {
        var result = await _userService.RegistrationAsync(request);

        if (!result.IsError)
        {
            UserDetails userToReturn = _mapper.Map<UserDetails>(result.Value);
            return CreatedAtRoute(
                "GetUser",
                new { Controller = "Users", id = userToReturn.Id },
                userToReturn
            ); // temp
        }
        return BadRequest(result.Errors[0].Description);
    }

    [HttpGet]
    [SwaggerResponse(200, "Claims have been validated", typeof(IEnumerable<UserListDetails>))]
    [SwaggerResponse(400)]
    [SwaggerResponse(500)]

    public async Task<IActionResult> GetAsync([FromQuery] UserParams userParams)
    {
        var users = await _userService.GetAsync(userParams);

        var usersToReturn = _mapper.Map<IEnumerable<UserListDetails>>(users);

        Response.AddPagination(
            users.CurrrentPage,
            users.PageSize,
            users.TotalCount,
            users.TotalPages
        );

        return Ok(usersToReturn);
    }

    [HttpGet("{id}", Name = "GetUser")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var user = await _userService.GetAsync(id);
        var userToReturn = _mapper.Map<UserDetails>(user.Value);

        return Ok(userToReturn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UserUpdateRequest request)
    {
        if (id != _currentUser.UserId)
        {
            return Unauthorized();
        }

        var result = await _userService.UpdateAsync(id, request);
        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }
        return NoContent();
    }

    [HttpPost("{id}/bookmark/{recipientId}")]
    public async Task<IActionResult> BookmakAsync(int id, int recipientId)
    {
        if (id != _currentUser.UserId)
        {
            return Unauthorized();
        }

        var result = await _userService.BookmakAsync(id, recipientId);

        if (result.IsError)
        {
            return BadRequest(result.Errors[0].Description);
        }
        return Ok();
    }
}
