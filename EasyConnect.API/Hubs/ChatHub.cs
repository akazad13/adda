using AutoMapper;
using EasyConnect.API.Data;
using EasyConnect.API.Dtos;
using EasyConnect.API.Models;
using EasyConnect.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace EasyConnect.API.Hubs;
[Authorize]
public class ChatHub(
    ICurrentUserService currentUserService,
     IMemberRepository repo,
    IMapper mapper
    ) : Hub
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IMemberRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    public override async Task OnConnectedAsync()
    {
        var sender = _currentUserService.UserId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{sender}");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var sender = $"{_currentUserService.UserId}";

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{sender}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(MessageForCreationDto createMessage)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId == createMessage.RecipientId)
            {
                throw new HubException("You cannot send messages to yourself!");
            }

            createMessage.SenderId = userId;

            var message = _mapper.Map<Message>(createMessage);
            message.MessageSent = DateTime.Now;
            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                await Clients.Group($"{userId}").SendAsync("NewMessage", messageToReturn);
                await Clients
                    .Group($"{createMessage.RecipientId}")
                    .SendAsync("NewMessage", messageToReturn);
            }

            await MakeRead(createMessage.RecipientId, userId);
        }
        catch (Exception exception)
        {
            throw new HubException(exception.Message);
        }
    }

    public async Task ReadThreadMessage(ReadMessageThread data)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId == data.RecipientId)
            {
                return;
            }
            await MakeRead(data.RecipientId, userId);
        }
        catch (Exception exception)
        {
            throw new HubException(exception.Message);
        }
    }

    #region Private Methods

    private async Task MakeRead(long senderId, long receiverId)
    {
        var unreadMessages = await _repo.GetWhere(
            x =>
                x.RecipientId == receiverId
                && x.SenderId == senderId
                && !x.IsRead);
        if (unreadMessages.Count > 0)
        {
            for (int i = 0; i < unreadMessages.Count; i++)
            {
                unreadMessages[i].IsRead = true;
            }

            _repo.UpdateRange(unreadMessages);
           await _repo.SaveAll();
        }
    }

    #endregion
}
