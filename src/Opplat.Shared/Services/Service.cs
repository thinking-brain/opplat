using System.Security;
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
public interface IService<T> where T : IEntity
{
    public Task<ServiceResponse<T>> Get(object id);
    public Task<ServiceResponse<T>> List(int? page, int? itemsPerPage);
    public Task<ServiceResponse<T>> Create(T entity, string user);
    public Task<ServiceResponse<T>> Update(T entity, string user);
    public Task<ServiceResponse<T>> Delete(T entity, string user);

    public Task<ServiceResponse<T>> Delete(object id, string user);
}

public class BaseService<T> where T : IEntity
{
    protected IRepository<T> _repo;

    public BaseService(IRepository<T> repo)
    {
        _repo = repo;
    }

    public async Task<ServiceResponse<T>> Get(object id)
    {
        var entity = await _repo.Find(id);
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

    public async Task<ServiceResponse<T>> List(int? page, int? itemsPerPage)
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

    public async Task<ServiceResponse<T>> Create(T entity, string user)
    {
        entity.User = user;
        var response = await _repo.Create(entity);
        var result = new ServiceResponse<T>
        {
            Status = response.IsOk ? ServiceStatus.Ok : ServiceStatus.Error,
            Message = response.Message,
            Value = entity,
        };
        return result;
    }

    public async Task<ServiceResponse<T>> Update(T entity, string user)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<T>> Delete(T entity, string user)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<T>> Delete(object id, string user)
    {
        throw new NotImplementedException();
    }
}
