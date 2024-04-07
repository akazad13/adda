using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EasyConnect.API.Data;
using EasyConnect.API.Dtos;
using EasyConnect.API.Helpers;
using EasyConnect.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyConnect.API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/users")]
public class UsersController(IMemberRepository repo, IMapper mapper) : ControllerBase
{
    private readonly IMemberRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        userParams.UserId = currentUserId;

        var users = await _repo.GetUsers(userParams);

        var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

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
        var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
        var user = await _repo.GetUser(id, isCurrentUser);

        var userToReturn = _mapper.Map<UserForDetailedDto>(user);

        return Ok(userToReturn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
    {
        if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        var userFromRepo = await _repo.GetUser(id, true);

        _mapper.Map(userForUpdateDto, userFromRepo); // (from, to)

        if (await _repo.SaveAll())
        {
            return NoContent();
        }
        return BadRequest($"Updating user {id} failed on save");
    }

    [HttpPost("{id}/like/{recipientId}")]
    public async Task<IActionResult> LikeUser(int id, int recipientId)
    {
        if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        var like = await _repo.GetLike(id, recipientId);

        if (like != null)
        {
            return BadRequest("You already bookmark this user.");
        }

        if (await _repo.GetUser(recipientId, false) == null)
        {
            return NotFound();
        }

        var newLike = new Like { LikerId = id, LikeeId = recipientId };

        _repo.Add<Like>(newLike);

        if (await _repo.SaveAll())
        {
            return Ok();
        }

        return BadRequest("Failed to like user");
    }
}
