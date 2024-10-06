using Adda.API.Data;

namespace Adda.API.Repositories;

public abstract class BaseRepository(DataContext context)
{
    private readonly DataContext _context = context;

    public async Task AddAsync<T>(T entity) where T : class => await _context.AddAsync(entity);

    public void Delete<T>(T entity) where T : class => _context.Remove(entity);

    public void UpdateRange<T>(IList<T> entities) where T : class => _context.UpdateRange(entities);
    public async Task<bool> SaveAllAsync() => await _context.SaveChangesAsync() > 0;
}
