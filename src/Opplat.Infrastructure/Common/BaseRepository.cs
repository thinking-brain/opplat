using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared;
using Microsoft.EntityFrameworkCore;
using Opplat.Shared.Repositories;
using Opplat.Shared.Entities;

namespace Opplat.Infrastructure.Common;

public class BaseRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly DbContext _db;

    public BaseRepository(DbContext db)
    {
        _db = db;
    }

    public async Task<RepositoryResponse> Create(T entity)
    {
        try
        {
            _db.Set<T>().Add(entity);
            await _db.SaveChangesAsync();
            var result = new RepositoryResponse
            {
                IsOk = true,
                Message = String.Format("Entity of type {0} created successfully.", typeof(T).ToString())
            };
            return result;
        }
        catch (System.Exception ex)
        {
            var result = new RepositoryResponse
            {
                IsOk = false,
                Message = String.Format("Error creating Entity of type {0}. {1}", typeof(T).ToString(), ex.Message)
            };
            return result;
        }
    }

    public async Task<RepositoryResponse> Delete(string id)
    {
        try
        {
            var entity = await _db.Set<T>().FindAsync(typeof(T), id);
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
            var result = new RepositoryResponse
            {
                IsOk = true,
                Message = String.Format("Entity of type {0} deleted successfully.", typeof(T).ToString())
            };
            return result;
        }
        catch (System.Exception ex)
        {
            var result = new RepositoryResponse
            {
                IsOk = false,
                Message = String.Format("Error deleting Entity of type {0}. {1}", typeof(T).ToString(), ex.Message)
            };
            return result;
        }
    }

    public async Task<RepositoryResponse> Delete(T entity)
    {
        try
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
            var result = new RepositoryResponse
            {
                IsOk = true,
                Message = String.Format("Entity of type {0} deleted successfully.", typeof(T).ToString())
            };
            return result;
        }
        catch (System.Exception ex)
        {
            var result = new RepositoryResponse
            {
                IsOk = false,
                Message = String.Format("Error deleting Entity of type {0}. {1}", typeof(T).ToString(), ex.Message)
            };
            return result;
        }
    }

    public async Task<T> Find(object id)
    {
        try
        {
            var entity = await _db.Set<T>().FindAsync(typeof(T), id);
            return entity;
        }
        catch (System.Exception ex)
        {
            return default(T);
        }
    }

    public async Task<IEnumerable<T>> List(int page = 0, int pageSize = 0)
    {
        try
        {
            // TODO: Implement paginations
            var list = await _db.Set<T>().ToListAsync();
            return list;
        }
        catch (System.Exception ex)
        {
            return new List<T>();
        }
    }

    public IQueryable<T> Query()
    {
        try
        {
            var result =  _db.Set<T>().AsQueryable();
            return result;
        }
        catch (System.Exception ex)
        {
            return null;
        }
    }

    public async Task<RepositoryResponse> Update(T entity)
    {
        try
        {
            _db.Set<T>().Update(entity);
            await _db.SaveChangesAsync();
            var result = new RepositoryResponse
            {
                IsOk = true,
                Message = String.Format("Entity of type {0} updated successfully.", typeof(T).ToString())
            };
            return result;
        }
        catch (System.Exception ex)
        {
            var result = new RepositoryResponse
            {
                IsOk = false,
                Message = String.Format("Error updating Entity of type {0}. {1}", typeof(T).ToString(), ex.Message)
            };
            return result;
        }
    }
}
