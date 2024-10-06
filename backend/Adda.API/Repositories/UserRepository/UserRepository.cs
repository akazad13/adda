using Adda.API.Data;
using Adda.API.Helpers;
using Adda.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Adda.API.Repositories.UserRepository;

public class UserRepository(DataContext context) : BaseRepository(context), IUserRepository
{
    private readonly DataContext _context = context;

    public async Task<User?> GetAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetAsync(int id, bool isCurrentUser)
    {
        if (isCurrentUser)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        else
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
        }
    }

    public async Task<PageList<User>> GetAsync(UserParams userParams)
    {
        IQueryable<User> users = _context.Users
            .Include(p => p.Photos)
            .OrderByDescending(u => u.LastActive)
            .AsQueryable();

        users = users.Where(u => u.Id != userParams.UserId);

        if (userParams.Bookmarkers)
        {
            IEnumerable<int> userBookmarks = await getUserBookmarksAsync(
                userParams.UserId,
                userParams.Bookmarkers
            );
            users = users.Where(u => userBookmarks.Contains(u.Id));
        }

        if (userParams.Bookmarkeds)
        {
            IEnumerable<int> userBookmarkeds = await getUserBookmarksAsync(
                userParams.UserId,
                userParams.Bookmarkers
            );
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

        return await PageList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
    }

    private async Task<IEnumerable<int>> getUserBookmarksAsync(int id, bool bookmarkers)
    {
        User user = await _context.Users
            .Include(x => x.Bookmarkers)
            .Include(x => x.Bookmarkeds)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return new List<int>();
        }

        if (bookmarkers)
        {
            return user.Bookmarkers.Where(u => u.BookmarkedId == id).Select(i => i.BookmarkerId);
        }
        else
        {
            return user.Bookmarkeds.Where(u => u.BookmarkerId == id).Select(i => i.BookmarkedId);
        }
    }
    public async Task<Bookmark?> GetBookmarkAsync(int userId, int recipientId)
    {
        return await _context.Bookmarks.FirstOrDefaultAsync(
            u => u.BookmarkerId == userId && u.BookmarkedId == recipientId
        );
    }

    public async Task<IEnumerable<object>> GetUsersWithRolesAsync() => await _context.Users
            .OrderBy(x => x.UserName)
            .Select(
                user =>
                    new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Roles = (
                            from userRole in user.UserRoles
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            select role.Name
                        ).ToList()
                    }
            )
            .ToListAsync();
}
