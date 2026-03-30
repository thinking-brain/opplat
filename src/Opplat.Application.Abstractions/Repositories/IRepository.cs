
using Opplat.Domain.Entities;

namespace Opplat.Application.Abstractions.Repositories;

public class RepositoryResponse
{
    public bool IsOk { get; set; }

    public string Message { get; set; } = string.Empty;
}
public interface IRepository<TEntity> where TEntity: IEntity
{

    Task<TEntity> Find(object id);
    Task<IEnumerable<TEntity>> List(int page = 0, int pageSize = 0);
    IQueryable<TEntity> Query();
    Task<RepositoryResponse> Create(TEntity entity);
    Task<RepositoryResponse> Update(TEntity entity);
    Task<RepositoryResponse> Delete(object id);
    Task<RepositoryResponse> Delete(TEntity entity);
}
