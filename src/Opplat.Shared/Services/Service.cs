using System.Runtime.CompilerServices;
using System.Security;
using Microsoft.Extensions.Logging;
using Opplat.Shared.Entities;
using Opplat.Shared.Repositories;

namespace Opplat.Shared.Services;

public enum ServiceStatus
{
    Ok, Error
}
public class ServiceResponse<T> where T : IEntity
{
    public ServiceStatus Status { get; set; }
    public T? Value { get; set; }
    public IEnumerable<T> List { get; set; }
    public string Message { get; set; }
}
public interface IService<T, K> where T : IEntity
{
    public Task<ServiceResponse<T>> Get(K id);
    public Task<ServiceResponse<T>> List(int? page = null, int? itemsPerPage = null);
    public Task<ServiceResponse<T>> Create(T entity, string user);
    public Task<ServiceResponse<T>> Update(T entity, string user);
    public Task<ServiceResponse<T>> DeleteEntity(T entity, string user);

    public Task<ServiceResponse<T>> Delete(K id, string user);
}

public class BaseService<T, K> where T : IEntity
{
    protected IRepository<T> _repo;
    protected ILogger<T> _logger;

    public BaseService(IRepository<T> repo,ILogger<T> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public virtual async Task<ServiceResponse<T>> Get(K id)
    {
        object idForSearch = id;
        if (id is string)
        {
            idForSearch = new Guid(id as string);
        }
        var entity = await _repo.Find(idForSearch);
        if (entity != null)
        {
            return new ServiceResponse<T>
            {
                Value = entity,
                Message = "Entity found.",
                Status = ServiceStatus.Ok,
            };
        }
        return new ServiceResponse<T>
        {
            Message = "Entity not found.",
            Status = ServiceStatus.Error,
        };
    }

    public virtual async Task<ServiceResponse<T>> List(int? page, int? itemsPerPage)
    {
        IEnumerable<T> list;
        if (page.HasValue && itemsPerPage.HasValue)
        {
            var query = _repo.Query();
            int previusItems = (page.Value - 1) * itemsPerPage.Value;
            list = query.Skip(previusItems).Take(itemsPerPage.Value);
        }
        else
        {
            list = await _repo.List();
        }
        var result = new ServiceResponse<T>
        {
            Status = ServiceStatus.Ok,
            Message = "List of entities.",
            List = list,
        };
        return result;
    }

    public virtual async Task<ServiceResponse<T>> Create(T entity, string user)
    {
        // entity.User = user;
        var response = await _repo.Create(entity);
        var result = new ServiceResponse<T>
        {
            Status = response.IsOk ? ServiceStatus.Ok : ServiceStatus.Error,
            Message = response.Message,
            Value = entity,
        };
        return result;
    }

    public virtual async Task<ServiceResponse<T>> Update(T entity, string user)
    {
        // entity.User = user;
        var response = await _repo.Update(entity);
        var result = new ServiceResponse<T>
        {
            Status = response.IsOk ? ServiceStatus.Ok : ServiceStatus.Error,
            Message = response.Message,
            Value = entity,
        };
        return result;
    }

    public virtual async Task<ServiceResponse<T>> DeleteEntity(T entity, string user)
    {
        var response = await _repo.Delete(entity);
        var result = new ServiceResponse<T>
        {
            Status = response.IsOk ? ServiceStatus.Ok : ServiceStatus.Error,
            Message = response.Message,
            Value = entity,
        };
        return result;
    }

    public virtual async Task<ServiceResponse<T>> Delete(K id, string user)
    {
        object idForSearch = id;
        if (id is string)
        {
            idForSearch = new Guid(id as string);
        }
        var response = await _repo.Delete(idForSearch);
        var result = new ServiceResponse<T>
        {
            Status = response.IsOk ? ServiceStatus.Ok : ServiceStatus.Error,
            Message = response.Message,
        };
        return result;
    }

    protected IQueryable<T> Query()
    {
        return _repo.Query();
    }
}
