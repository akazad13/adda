using System.Collections.Generic;
using System.Threading.Tasks;
using EasyConnect.API.Models;

namespace EasyConnect.API.Data
{
    public interface IAdminRepository
    {
        Task<IEnumerable<object>> GetUsersWithRoles();
        Task<IEnumerable<object>> GetAllPhotos();
        Task<Photo> GetPhoto(int photoId);
        Task<bool> SaveAll();
        void Delete<T>(T entity) where T : class;
    }
}
