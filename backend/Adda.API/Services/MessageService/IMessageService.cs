using Adda.API.Helpers;
using Adda.API.Models;
using ErrorOr;

namespace Adda.API.Services.MessageService;

public interface IMessageService
{
    Task<Message> GetAsync(int id);
    Task<PageList<Message>> GetMessagesForUserAsync(MessageParams messageParams);
    Task<IEnumerable<Message>> GetMessageThreadAsync(int userId, int recipientId);
    Task<ErrorOr<Success>> DeleteAsync(int userId, int id);
}
