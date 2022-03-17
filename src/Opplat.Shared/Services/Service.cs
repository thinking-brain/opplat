using System.Security;
using Opplat.Shared.Entities;

namespace Opplat.Shared.Services;

public enum ServiceStatus
{
    Ok, Error
}
public interface IServiceResponse<T> where T: IEntity
{
    public ServiceStatus Status { get; set; }
    public T? Entidad { get; set; }
    public IEnumerable<T> Listado { get; set; }
    public string Message { get; set; }
}
public interface IService<T> where T: IEntity
{
    public Task<IServiceResponse<T>> Get();
    public Task<IServiceResponse<T>> List(int? page, int? itemsPerPage);
    public Task<IServiceResponse<T>> Create(T entity);
    public Task<IServiceResponse<T>> Update(T entity);
    public Task<IServiceResponse<T>> Delete(T entity);
}
