using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Adda.API.Data;
using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Adda.API.Controllers;

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

    public async Task<IActionResult> GetAsync([FromQuery] UserParams userParams)
    {
        int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        userParams.UserId = currentUserId;

        PageList<User> users = await _repo.GetUsersAsync(userParams);

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
    public async Task<IActionResult> GetAsync(int id)
    {
        bool isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
        User user = await _repo.GetUserAsync(id, isCurrentUser);

        UserForDetailedDto userToReturn = _mapper.Map<UserForDetailedDto>(user);

        return Ok(userToReturn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UserForUpdateDto userForUpdateDto)
    {
        if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        User userFromRepo = await _repo.GetUserAsync(id, true);

        _mapper.Map(userForUpdateDto, userFromRepo); // (from, to)

        if (await _repo.SaveAllAsync())
        {
            return NoContent();
        }
        return BadRequest($"Updating user {id} failed on save");
    }

    [HttpPost("{id}/bookmark/{recipientId}")]
    public async Task<IActionResult> BookmakAsync(int id, int recipientId)
    {
        if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        Bookmark bookmark = await _repo.GetBookmarkAsync(id, recipientId);

        if (bookmark != null)
        {
            return BadRequest("You already bookmark this user.");
        }

        if (await _repo.GetUserAsync(recipientId, false) == null)
        {
            return NotFound();
        }

        var newBookmark = new Bookmark { BookmarkerId = id, BookmarkedId = recipientId };

        _repo.Add<Bookmark>(newBookmark);

        if (await _repo.SaveAllAsync())
        {
            return Ok();
        }

        return BadRequest("Failed to bookmark user");
    }
}
