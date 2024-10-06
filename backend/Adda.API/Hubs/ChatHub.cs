using Adda.API.Dtos;
using Adda.API.Models;
using Adda.API.Repositories.MessageRepository;
using Adda.API.Security.CurrentUserProvider;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Adda.API.Hubs;
[Authorize]
public class ChatHub(
    ICurrentUserProvider currentUser,
    IMessageRepository messageRepository,
    IMapper mapper
    ) : Hub
{
    private readonly ICurrentUserProvider _currentUser = currentUser;
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IMapper _mapper = mapper;

    public override async Task OnConnectedAsync()
    {
        int sender = _currentUser.UserId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{sender}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string sender = $"{_currentUser.UserId}";

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{sender}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageAsync(CreateMessageRequest createMessage)
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
            await _messageRepository.AddAsync(message);

            if (await _messageRepository.SaveAllAsync())
            {
                MessageResponse messageToReturn = _mapper.Map<MessageResponse>(message);
                await Clients.Group($"{userId}").SendAsync("NewMessage", messageToReturn);
                await Clients
                    .Group($"{createMessage.RecipientId}")
                    .SendAsync("NewMessage", messageToReturn);
            }

            await makeReadAsync(createMessage.RecipientId, userId);
        }
        catch (Exception exception)
        {
            throw new HubException(exception.Message);
        }
    }

    public async Task ReadThreadMessageAsync(ReadMessageThread data)
    {
        try
        {
            int userId = _currentUser.UserId;
            if (userId == data.RecipientId)
            {
                return;
            }
            await makeReadAsync(data.RecipientId, userId);
        }
        catch (Exception exception)
        {
            throw new HubException(exception.Message);
        }
    }

    #region Private Methods

    private async Task makeReadAsync(long senderId, long receiverId)
    {
        List<Message> unreadMessages = await _messageRepository.GetWhereAsync(
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

            _messageRepository.UpdateRange(unreadMessages);
            await _messageRepository.SaveAllAsync();
        }
    }

    #endregion
}
