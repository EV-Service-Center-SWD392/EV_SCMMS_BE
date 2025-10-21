using System.Linq.Expressions;

namespace EV_SCMMS.Core.Application.Interfaces.Repositories;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IGenericRepository<T> where T : class
{
    // Sync methods
    List<T> GetAll();
    T GetById(int id);
    T GetById(string code);
    T GetById(Guid code);
    void Add(T entity);
    void Update(T entity);
    bool Remove(T entity);
    
    // Async methods
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> GetByIdAsync(string code);
    Task<T> GetByIdAsync(Guid code);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> RemoveAsync(T entity);
    
    // Prepare methods (without save)
    void PrepareCreate(T entity);
    void PrepareUpdate(T entity);
    void PrepareRemove(T entity);
    int Save();
    Task<int> SaveAsync();
    
    // Queryable
    IQueryable<T> GetAllQueryable();
}
