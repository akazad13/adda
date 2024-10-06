using System.Linq.Expressions;
using Adda.API.Data;
using Adda.API.Helpers;
using Adda.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Adda.API.Repositories.MessageRepository;

public class MessageRepository(DataContext context) : BaseRepository(context), IMessageRepository
{
    private readonly DataContext _context = context;

    public async Task<Message> GetMessageAsync(int id) => await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);

    public async Task<PageList<Message>> GetMessagesForUserAsync(MessageParams messageParams)
    {
        IQueryable<Message> messages = _context.Messages
            .Include(m => m.Sender)
            .ThenInclude(p => p.Photos)
            .Include(m => m.Recipient)
            .ThenInclude(p => p.Photos)
            .AsQueryable();

        messages = messageParams.MessageContainer switch
        {
            "Inbox" => messages.Where(
                                m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted
                            ),
            "Outbox" => messages.Where(
                                m => m.SenderId == messageParams.UserId && !m.SenderDeleted
                            ),
            _ => messages.Where(
                                m =>
                                    m.RecipientId == messageParams.UserId
                                    && !m.IsRead
                                    && !m.RecipientDeleted
                            ),
        };
        messages = messages.OrderByDescending(m => m.MessageSent);

        return await PageList<Message>.CreateAsync(
            messages,
            messageParams.PageNumber,
            messageParams.PageSize
        );
    }


    public async Task<IEnumerable<Message>> GetMessageThreadAsync(int userId, int recipientId)
    {
        List<Message> messages = await _context.Messages
            .Include(m => m.Sender)
            .ThenInclude(p => p.Photos)
            .Include(m => m.Recipient)
            .ThenInclude(p => p.Photos)
            .Where(
                m =>
                    m.RecipientId == userId
                        && !m.RecipientDeleted
                        && m.SenderId == recipientId
                    || m.RecipientId == recipientId
                        && !m.SenderDeleted
                        && m.SenderId == userId
            )
            .OrderBy(m => m.MessageSent)
            .ToListAsync();
        return messages;
    }

    public async Task<List<Message>> GetWhereAsync(Expression<Func<Message, bool>> expression) => await _context.Messages.Where(expression).ToListAsync();
}
