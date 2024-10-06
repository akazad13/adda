using System.Collections.Generic;
using System.Threading.Tasks;
using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Security.CurrentUserProvider;
using Adda.API.Services.UserService;
using AutoMapper;
using ErrorOr;
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

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegistrationRequest request)
    {
        ErrorOr<User> user = await _userService.RegistrationAsync(request);

        if (!user.IsError)
        {
            UserDetails userToReturn = _mapper.Map<UserDetails>(user);
            return CreatedAtRoute(
                "GetUser",
                new { Controller = "Users", id = userToReturn.Id },
                userToReturn
            ); // temp
        }
        return BadRequest(user.Errors);
    }

    [HttpGet]
    [SwaggerResponse(200, "Claims have been validated", typeof(IEnumerable<UserListDetails>))]
    [SwaggerResponse(400)]
    [SwaggerResponse(500)]

    public async Task<IActionResult> GetAsync([FromQuery] UserParams userParams)
    {
        PageList<User> users = await _userService.GetAsync(userParams);

        IEnumerable<UserListDetails> usersToReturn = _mapper.Map<IEnumerable<UserListDetails>>(users);

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
        ErrorOr<User> user = await _userService.GetAsync(id);
        UserDetails userToReturn = _mapper.Map<UserDetails>(user.Value);

        return Ok(userToReturn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UserUpdateRequest request)
    {
        if (id != _currentUser.UserId)
        {
            return Unauthorized();
        }

        ErrorOr<Success> result = await _userService.UpdateAsync(id, request);
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

        ErrorOr<Success> result = await _userService.BookmakAsync(id, recipientId);

        if (result.IsError)
        {
            return BadRequest("Failed to bookmark user");
        }
        return Ok();
    }
}
