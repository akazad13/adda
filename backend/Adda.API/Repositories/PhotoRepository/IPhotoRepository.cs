using Adda.API.Models;

namespace Adda.API.Repositories.PhotoRepository;

public interface IPhotoRepository
{
    Task AddAsync<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void UpdateRange<T>(IList<T> entities) where T : class;
    Task<bool> SaveAllAsync();
    Task<Photo> GetAsync(int id);
    Task<Photo> GetMainPhotoForUserAsync(int userId);
    Task<IEnumerable<object>> GetAllPhotosAsync();
    Task<Photo> GetPhotoAsync(int photoId);
}
