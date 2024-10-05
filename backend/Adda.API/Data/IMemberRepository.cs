using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adda.API.Helpers;
using Adda.API.Models;

namespace Adda.API.Data;

public interface IMemberRepository
{
    Task AddAsync<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void UpdateRange<T>(IList<T> entities) where T : class;
    Task<bool> SaveAllAsync();
    Task<PageList<User>> GetUsersAsync(UserParams userParams);
    Task<User> GetUserAsync(int id, bool isCurrentUser);
    Task<Photo> GetPhotoAsync(int id);
    Task<Photo> GetMainPhotoForUserAsync(int userId);
    Task<Bookmark> GetBookmarkAsync(int userId, int recipientId);
    Task<Message> GetMessageAsync(int id);
    Task<PageList<Message>> GetMessagesForUserAsync(MessageParams messageParams);
    Task<IEnumerable<Message>> GetMessageThreadAsync(int userId, int recipientId);
    Task<List<Message>> GetWhereAsync(Expression<Func<Message, bool>> expression);
}
