using System.Collections.Generic;
using System.Threading.Tasks;
using EasyConnect.API.Models;

namespace EasyConnect.API.Data;

public interface IAdminRepository
{
    Task<IEnumerable<object>> GetUsersWithRolesAsync();
    Task<IEnumerable<object>> GetAllPhotosAsync();
    Task<Photo> GetPhotoAsync(int photoId);
    Task<bool> SaveAllAsync();
    void Delete<T>(T entity) where T : class;
}
