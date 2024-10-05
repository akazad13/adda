using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adda.API.Helpers;
using Adda.API.Models;

namespace Adda.API.Repositories.MessageRepository;

public interface IMessageRepository
{
    Task AddAsync<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void UpdateRange<T>(IList<T> entities) where T : class;
    Task<bool> SaveAllAsync();
    Task<Message> GetMessageAsync(int id);
    Task<PageList<Message>> GetMessagesForUserAsync(MessageParams messageParams);
    Task<IEnumerable<Message>> GetMessageThreadAsync(int userId, int recipientId);
    Task<List<Message>> GetWhereAsync(Expression<Func<Message, bool>> expression);
}
