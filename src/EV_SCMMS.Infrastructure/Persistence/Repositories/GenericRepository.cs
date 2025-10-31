
using EV_SCMMS.Core.Application.Interfaces.Repositories;
using EV_SCMMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EV_SCMMS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic repository implementation with common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected AppDbContext _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _dbSet = context;
    }

    public List<T> GetAll()
    {
        return _dbSet.Set<T>().ToList();
    }
    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.Set<T>().ToListAsync();
    }
    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public void Update(T entity)
    {
        var tracker = _dbSet.Attach(entity);
        tracker.State = EntityState.Modified;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        var tracker = _dbSet.Attach(entity);
        tracker.State = EntityState.Modified;

        return entity;
    }

    public bool Remove(T entity)
    {
        _dbSet.Remove(entity);

        return true;
    }

    public async Task<bool> RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _dbSet.SaveChangesAsync();

        return true;
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        var entity = await _dbSet.Set<T>().FindAsync(id);
        if (entity != null)
        {
            // Đảm bảo không bị tracking lại
            _dbSet.Entry(entity).State = EntityState.Detached;
        }
        return entity;
    }

    #region Separating asigned entity and save operators        

    public void PrepareCreate(T entity)
    {
        _dbSet.Add(entity);
    }

    public void PrepareUpdate(T entity)
    {
        var tracker = _dbSet.Attach(entity);
        tracker.State = EntityState.Modified;
    }

    public void PrepareRemove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public int Save()
    {
        return _dbSet.SaveChanges();
    }

    public async Task<int> SaveAsync()
    {
        return await _dbSet.SaveChangesAsync();
    }

    #endregion Separating asign entity and save operators

    public IQueryable<T> GetAllQueryable()
    {
        return _dbSet.Set<T>().AsQueryable();
    }
}
