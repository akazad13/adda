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
    IUserService userService,
    ICurrentUserProvider currentUser
) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IUserService _userService = userService;
    private readonly ICurrentUserProvider _currentUser = currentUser;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(UserForRegisterDto userForRegisterDto)
    {
        ErrorOr<User> user = await _userService.RegistrationAsync(userForRegisterDto);

        if (!user.IsError)
        {
            UserForDetailedDto userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return CreatedAtRoute(
                "GetUser",
                new { Controller = "Users", id = userToReturn.Id },
                userToReturn
            ); // temp
        }
        return BadRequest(user.Errors);
    }

    [HttpGet]
    [SwaggerResponse(200, "Claims have been validated", typeof(IEnumerable<UserForListDto>))]
    [SwaggerResponse(400)]
    [SwaggerResponse(500)]

    public async Task<IActionResult> GetAsync([FromQuery] UserParams userParams)
    {
        ErrorOr<PageList<User>> users = await _userService.GetAsync(userParams);
        if (users.IsError)
        {
            return BadRequest(users.Errors);
        }
        IEnumerable<UserForListDto> usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users.Value);

        Response.AddPagination(
            users.Value.CurrrentPage,
            users.Value.PageSize,
            users.Value.TotalCount,
            users.Value.TotalPages
        );

        return Ok(usersToReturn);
    }

    [HttpGet("{id}", Name = "GetUser")]
    public async Task<IActionResult> GetAsync(int id)
    {
        ErrorOr<User> user = await _userService.GetAsync(id);
        UserForDetailedDto userToReturn = _mapper.Map<UserForDetailedDto>(user.Value);

        return Ok(userToReturn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UserForUpdateDto userForUpdateDto)
    {
        if (id != _currentUser.UserId)
        {
            return Unauthorized();
        }

        ErrorOr<Success> result = await _userService.UpdateAsync(id, userForUpdateDto);
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
