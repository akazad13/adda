using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EasyConnect.API.Data;
using EasyConnect.API.Dtos;
using EasyConnect.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EasyConnect.API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/users/{userId}/messages")]
public class MessagesController(IMemberRepository repo, IMapper mapper) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    public IMemberRepository _repo { get; set; } = repo;

    [HttpGet("{id}", Name = "GetMessage")]
    public async Task<IActionResult> GetMessage(int userId, int id)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        Models.Message messageFromRepo = await _repo.GetMessage(id);

        if (messageFromRepo == null)
        {
            return NoContent();
        }

        return Ok(messageFromRepo);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessagesForUser(
        int userId,
        [FromQuery] MessageParams messageParams
    )
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        messageParams.UserId = userId;

        PageList<Models.Message> messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
        IEnumerable<MessageToReturnDto> messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
        Response.AddPagination(
            messagesFromRepo.CurrrentPage,
            messagesFromRepo.PageSize,
            messagesFromRepo.TotalCount,
            messagesFromRepo.TotalPages
        );
        return Ok(messages);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        IEnumerable<Models.Message> messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);
        IEnumerable<MessageToReturnDto> messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

        return Ok(messages);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> DeleteMessage(int userId, int id)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
            return Unauthorized();
        }

        Models.Message messageFromRepo = await _repo.GetMessage(id);

        if (messageFromRepo == null)
        {
            return BadRequest("Could not find user");
        }

        if (messageFromRepo.SenderId == userId)
        {
            messageFromRepo.SenderDeleted = true;
        }
        if (messageFromRepo.RecipientId == userId)
        {
            messageFromRepo.RecipientDeleted = true;
        }

        if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
        {
            _repo.Delete(messageFromRepo);
        }

        if (await _repo.SaveAll())
        {
            return NoContent();
        }

        return BadRequest("Error deleting the message");
    }
}
