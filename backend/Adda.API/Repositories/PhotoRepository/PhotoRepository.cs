using Adda.API.Data;
using Adda.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Adda.API.Repositories.PhotoRepository;

public class PhotoRepository(DataContext context) : BaseRepository(context), IPhotoRepository
{
    private readonly DataContext _context = context;

    public async Task<Photo> GetMainPhotoForUserAsync(int userId)
    {
        return await _context.Photos
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
    }

    public async Task<Photo> GetAsync(int id)
    {
        return await _context.Photos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<object>> GetAllPhotosAsync() => await _context.Photos
            .IgnoreQueryFilters()
            .Where(p => !p.IsApproved)
            .Select(
                u =>
                    new
                    {
                        Id = u.Id,
                        UserName = u.User.UserName,
                        Url = u.Url,
                        IsApproved = u.IsApproved
                    }
            )
            .ToListAsync();

    public async Task<Photo> GetPhotoAsync(int photoId) => await _context.Photos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == photoId);

}
