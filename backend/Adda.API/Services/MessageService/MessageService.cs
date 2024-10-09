using Adda.API.Helpers;
using Adda.API.Models;
using Adda.API.Repositories.MessageRepository;
using ErrorOr;

namespace Adda.API.Services.MessageService;

public class MessageService(IMessageRepository messageRepository) : IMessageService
{
    private readonly IMessageRepository _messageRepository = messageRepository;

    public async Task<Message> GetAsync(int id)
    {
        return await _messageRepository.GetMessageAsync(id);
    }

    public async Task<PageList<Message>> GetMessagesForUserAsync(MessageParams messageParams)
    {
        return await _messageRepository.GetMessagesForUserAsync(messageParams);
    }

    public async Task<IEnumerable<Message>> GetMessageThreadAsync(int userId, int recipientId)
    {

        return await _messageRepository.GetMessageThreadAsync(userId, recipientId);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(int userId, int id)
    {

        var messageFromRepo = await _messageRepository.GetMessageAsync(id);

        if (messageFromRepo == null)
        {
            return Error.Validation(description: "Could not find user");
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
            _messageRepository.Delete(messageFromRepo);
        }

        if (await _messageRepository.SaveAllAsync())
        {
            return Result.Success;
        }

        return Error.Failure(description: "Error deleting the message");
    }
}
