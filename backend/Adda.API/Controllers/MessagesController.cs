using Adda.API.Dtos;
using Adda.API.Helpers;
using Adda.API.Security.CurrentUserProvider;
using Adda.API.Services.MessageService;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Adda.API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/users/{userId}/messages")]
public class MessagesController(ICurrentUserProvider currentUser, IMessageService messageService) : ControllerBase
{
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IMessageService _messageService = messageService;

    [HttpGet("{id}", Name = "GetMessage")]
    public async Task<IActionResult> GetAsync(int userId, int id)
    {
        if (userId != _currentUser.UserId)
        {
            return Unauthorized();
        }

        var messageFromRepo = await _messageService.GetAsync(id);

        if (messageFromRepo == null)
        {
            return NoContent();
        }

        return Ok(messageFromRepo);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessagesForUserAsync(
        int userId,
        [FromQuery] MessageParams messageParams
    )
    {
        if (userId != _currentUser.UserId)
        {
            return Unauthorized();
        }

        messageParams.UserId = userId;

        var messagesFromRepo = await _messageService.GetMessagesForUserAsync(messageParams);
        var messages = messagesFromRepo.Adapt<IEnumerable<MessageResponse>>();
        Response.AddPagination(
            messagesFromRepo.CurrrentPage,
            messagesFromRepo.PageSize,
            messagesFromRepo.TotalCount,
            messagesFromRepo.TotalPages
        );
        return Ok(messages);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<IActionResult> GetMessageThreadAsync(int userId, int recipientId)
    {
        if (userId != _currentUser.UserId)
        {
            return Unauthorized();
        }

        var messagesFromRepo = await _messageService.GetMessageThreadAsync(userId, recipientId);
        var messages = messagesFromRepo.Adapt<IEnumerable<MessageResponse>>();

        return Ok(messages);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> DeleteAsync(int userId, int id)
    {
        if (userId != _currentUser.UserId)
        {
            return Unauthorized();
        }

        var result = await _messageService.DeleteAsync(userId, id);

        if (!result.IsError)
        {
            return NoContent();
        }

        return BadRequest("Error deleting the message");
    }
}
