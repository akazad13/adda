using Adda.API.Models;
using ErrorOr;

namespace Adda.API.Services.PhotoService;

public interface IPhotoService
{
    Task<ErrorOr<Photo>> AddAsync(int userId, IFormFile file);
    Task<ErrorOr<Photo>> GetAsync(int id);
    Task<ErrorOr<Success>> SetMainPhotoAsync(int userId, int id);
    Task<ErrorOr<Success>> DeleteAsync(int userId, int id);
    Task<ErrorOr<Success>> DeleteAsync(int photoId);
    Task<IEnumerable<object>> GetAllPhotosAsync();
    Task<Photo> GetPhotoAsync(int photoId);
    Task<ErrorOr<Success>> ApprovePhotoAsync(int photoId);
}
