using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adda.API.Helpers;
using Adda.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Adda.API.Data;

public class MemberRepository(DataContext context) : IMemberRepository
{
    private readonly DataContext _context = context;

    public void Add<T>(T entity) where T : class => _context.Add(entity);

    public void Delete<T>(T entity) where T : class => _context.Remove(entity);

    public void UpdateRange<T>(IList<T> entities) where T : class => _context.UpdateRange(entities);


    public async Task<Bookmark> GetBookmarkAsync(int userId, int recipientId) => await _context.Bookmarks.FirstOrDefaultAsync(
            u => u.BookmarkerId == userId && u.BookmarkedId == recipientId
        );

    public async Task<Photo> GetMainPhotoForUserAsync(int userId) => await _context.Photos
            .Where(u => u.UserId == userId)
            .FirstOrDefaultAsync(p => p.IsMain);

    public async Task<Photo> GetPhotoAsync(int id)
    {
        Photo photo = await _context.Photos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id);
        return photo;
    }

    public async Task<User> GetUserAsync(int id, bool isCurrentUser)
    {
        User user;
        if (isCurrentUser)
        {
            user = await _context.Users
                .Include(p => p.Photos)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        else
        {
            user = await _context.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        return user;
    }

    public async Task<PageList<User>> GetUsersAsync(UserParams userParams)
    {
        IQueryable<User> users = _context.Users
            .Include(p => p.Photos)
            .OrderByDescending(u => u.LastActive)
            .AsQueryable();

        users = users.Where(u => u.Id != userParams.UserId);

        if (userParams.Bookmarkers)
        {
            IEnumerable<int> userBookmarks = await getUserBookmarksAsync(userParams.UserId, userParams.Bookmarkers);
            users = users.Where(u => userBookmarks.Contains(u.Id));
        }

        if (userParams.Bookmarkeds)
        {
            IEnumerable<int> userBookmarkeds = await getUserBookmarksAsync(userParams.UserId, userParams.Bookmarkers);
            users = users.Where(u => userBookmarkeds.Contains(u.Id));
        }

        if (userParams.MinAge != 18 || userParams.MaxAge != 99)
        {
            DateTime minDob = DateTime.Today.AddYears(-userParams.MaxAge);
            DateTime maxDob = DateTime.Today.AddYears(-userParams.MinAge + 1);
            users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
        }

        if (!string.IsNullOrEmpty(userParams.OrderBy))
        {
            switch (userParams.OrderBy)
            {
                case "created":
                    users = users.OrderByDescending(u => u.Created);
                    break;
                default:
                    break;
            }
        }

        return await PageList<User>.CreateAsync(
            users,
            userParams.PageNumber,
            userParams.PageSize
        );
    }

    private async Task<IEnumerable<int>> getUserBookmarksAsync(int id, bool bookmarkers)
    {
        User user = await _context.Users
            .Include(x => x.Bookmarkers)
            .Include(x => x.Bookmarkeds)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (bookmarkers)
        {
            return user.Bookmarkers.Where(u => u.BookmarkedId == id).Select(i => i.BookmarkerId);
        }
        else
        {
            return user.Bookmarkeds.Where(u => u.BookmarkerId == id).Select(i => i.BookmarkedId);
        }
    }

    public async Task<bool> SaveAllAsync() => await _context.SaveChangesAsync() > 0;

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
