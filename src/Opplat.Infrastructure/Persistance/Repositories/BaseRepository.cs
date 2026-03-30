using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories;
using Opplat.Domain.Entities;

namespace Opplat.Infrastructure.Persistance.Repositories;

public class BaseRepository<T>(DbContext db, ILogger<IRepository<T>> logger) : IRepository<T> where T : class, IEntity
{
    protected readonly DbContext _db = db;
    protected ILogger<IRepository<T>> _logger = logger;

    public virtual async Task<RepositoryResponse> Create(T entity)
    {
        try
        {
            _db.Set<T>().Add(entity);
            await _db.SaveChangesAsync();
            var result = new RepositoryResponse
            {
                IsOk = true,
                Message = string.Format("Entity of type {0} created successfully.", typeof(T).Name)
            };
            return result;
        }
        catch (System.Exception ex)
        {
            var result = new RepositoryResponse
            {
                IsOk = false,
                Message = string.Format("Error creating Entity of type {0}. {1}", typeof(T).Name, ex.Message)
            };
            return result;
        }
    }

    public virtual async Task<RepositoryResponse> Delete(object id)
    {
        try
        {
            var entity = await _db.Set<T>().FindAsync(id);
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
            var result = new RepositoryResponse
            {
                IsOk = true,
                Message = string.Format("Entity of type {0} deleted successfully.", typeof(T).Name)
            };
            return result;
        }
        catch (System.Exception ex)
        {
            var result = new RepositoryResponse
            {
                IsOk = false,
                Message = string.Format("Error deleting Entity of type {0}. {1}", typeof(T).Name, ex.Message)
            };
            return result;
        }
    }

    public virtual async Task<RepositoryResponse> Delete(T entity)
    {
        try
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
            var result = new RepositoryResponse
            {
                IsOk = true,
                Message = string.Format("Entity of type {0} deleted successfully.", typeof(T).Name)
            };
            return result;
        }
        catch (System.Exception ex)
        {
            var result = new RepositoryResponse
            {
                IsOk = false,
                Message = string.Format("Error deleting Entity of type {0}. {1}", typeof(T).Name, ex.Message)
            };
            return result;
        }
    }

    public virtual async Task<T> Find(object id)
    {
        try
        {
            var entity = await _db.Set<T>().FindAsync(id);
            return entity;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error finding entity of type {EntityType}.", typeof(T).Name);
            return default;
        }
    }

    public virtual async Task<IEnumerable<T>> List(int page = 0, int pageSize = 0)
    {
        try
        {
            // TODO: Implement paginations
            var list = await _db.Set<T>().ToListAsync();
            return list;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error listing entities of type {EntityType}.", typeof(T).Name);
            return [];
        }
    }

    public virtual IQueryable<T> Query()
    {
        try
        {
            var result =  _db.Set<T>().AsQueryable();
            return result;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error querying entities of type {EntityType}.", typeof(T).Name);
            return Enumerable.Empty<T>().AsQueryable();
        }
    }

    public virtual async Task<RepositoryResponse> Update(T entity)
    {
        try
        {
            _db.Set<T>().Update(entity);
            await _db.SaveChangesAsync();
            var result = new RepositoryResponse
            {
                IsOk = true,
                Message = string.Format("Entity of type {0} updated successfully.", typeof(T).Name)
            };
            return result;
        }
        catch (System.Exception ex)
        {
            var result = new RepositoryResponse
            {
                IsOk = false,
                Message = string.Format("Error updating Entity of type {0}. {1}", typeof(T).Name, ex.Message)
            };
            return result;
        }
    }
}
