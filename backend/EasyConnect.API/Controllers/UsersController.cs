using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EasyConnect.API.Data;
using EasyConnect.API.Dtos;
using EasyConnect.API.Helpers;
using EasyConnect.API.Models;
using EasyConnect.API.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EasyConnect.API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/users")]
public class UsersController(
    IMemberRepository repo,
    IMapper mapper,
    IUserService userService) : ControllerBase
{
    private readonly IMemberRepository _repo = repo;
    private readonly IMapper _mapper = mapper;
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(UserForRegisterDto userForRegisterDto)
    {
        ErrorOr.ErrorOr<User> user = await _userService.RegistrationAsync(userForRegisterDto);

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

    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
    {
        int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        userParams.UserId = currentUserId;

        PageList<User> users = await _repo.GetUsers(userParams);

        IEnumerable<UserForListDto> usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

        Response.AddPagination(
            users.CurrrentPage,
            users.PageSize,
            users.TotalCount,
            users.TotalPages
        );

        return Ok(usersToReturn);
    }

    [HttpGet("{id}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(int id)
    {
        bool isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
        User user = await _repo.GetUser(id, isCurrentUser);

        UserForDetailedDto userToReturn = _mapper.Map<UserForDetailedDto>(user);

        return Ok(userToReturn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
    {
        if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        User userFromRepo = await _repo.GetUser(id, true);

        _mapper.Map(userForUpdateDto, userFromRepo); // (from, to)

        if (await _repo.SaveAll())
        {
            return NoContent();
        }
        return BadRequest($"Updating user {id} failed on save");
    }

    [HttpPost("{id}/bookmark/{recipientId}")]
    public async Task<IActionResult> BookmakUser(int id, int recipientId)
    {
        if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        Bookmark bookmark = await _repo.GetBookmark(id, recipientId);

        if (bookmark != null)
        {
            return BadRequest("You already bookmark this user.");
        }

        if (await _repo.GetUser(recipientId, false) == null)
        {
            return NotFound();
        }

        var newBookmark = new Bookmark { BookmarkerId = id, BookmarkedId = recipientId };

        _repo.Add<Bookmark>(newBookmark);

        if (await _repo.SaveAll())
        {
            return Ok();
        }

        return BadRequest("Failed to bookmark user");
    }
}
