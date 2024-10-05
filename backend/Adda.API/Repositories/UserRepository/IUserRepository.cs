using System.Collections.Generic;
using System.Threading.Tasks;
using Adda.API.Helpers;
using Adda.API.Models;

namespace Adda.API.Repositories.UserRepository;

public interface IUserRepository
{
    Task AddAsync<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void UpdateRange<T>(IList<T> entities) where T : class;
    Task<bool> SaveAllAsync();
    Task<User> GetAsync(int id, bool isCurrentUser);
    Task<PageList<User>> GetAsync(UserParams userParams);
    Task<Bookmark> GetBookmarkAsync(int userId, int recipientId);
    Task<IEnumerable<object>> GetUsersWithRolesAsync();
}
