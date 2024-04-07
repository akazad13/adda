using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyConnect.API.Helpers;
using EasyConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyConnect.API.Data
{
    public class MemberRepository(DataContext context) : IMemberRepository
    {
        private readonly DataContext _context = context;

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(
                u => u.LikerId == userId && u.LikeeId == recipientId
            );
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
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

        public async Task<PageList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users
                .Include(p => p.Photos)
                .OrderByDescending(u => u.LastActive)
                .AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge + 1);
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

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
                .Include(x => x.Likers)
                .Include(x => x.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PageList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .Include(m => m.Sender)
                .ThenInclude(p => p.Photos)
                .Include(m => m.Recipient)
                .ThenInclude(p => p.Photos)
                .AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(
                        m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted
                    );
                    break;
                case "Outbox":
                    messages = messages.Where(
                        m => m.SenderId == messageParams.UserId && !m.SenderDeleted
                    );
                    break;
                default:
                    messages = messages.Where(
                        m =>
                            m.RecipientId == messageParams.UserId
                            && !m.IsRead
                            && !m.RecipientDeleted
                    );
                    break;
            }

            messages = messages.OrderByDescending(m => m.MessageSent);

            return await PageList<Message>.CreateAsync(
                messages,
                messageParams.PageNumber,
                messageParams.PageSize
            );
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
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
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();
            return messages;
        }
    }
}
