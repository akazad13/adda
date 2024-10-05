using System;
using System.Threading.Tasks;
using AutoMapper;
using EasyConnect.API.Data;
using EasyConnect.API.Dtos;
using EasyConnect.API.Models;
using EasyConnect.API.Security.CurrentUserProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EasyConnect.API.Hubs;
[Authorize]
public class ChatHub(
    ICurrentUserProvider currentUser,
     IMemberRepository repo,
    IMapper mapper
    ) : Hub
{
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IMemberRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    public override async Task OnConnectedAsync()
    {
        int sender = _currentUser.UserId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{sender}");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        string sender = $"{_currentUser.UserId}";

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{sender}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(MessageForCreationDto createMessage)
    {
        try
        {
            int userId = _currentUser.UserId;
            if (userId == createMessage.RecipientId)
            {
                throw new HubException("You cannot send messages to yourself!");
            }

            createMessage.SenderId = userId;

            Message message = _mapper.Map<Message>(createMessage);
            message.MessageSent = DateTime.Now;
            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                MessageToReturnDto messageToReturn = _mapper.Map<MessageToReturnDto>(message);
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
            int userId = _currentUser.UserId;
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
        System.Collections.Generic.List<Message> unreadMessages = await _repo.GetWhere(
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
