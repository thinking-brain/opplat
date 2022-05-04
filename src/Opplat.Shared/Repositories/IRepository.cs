
using Opplat.Shared.Entities;

namespace Opplat.Shared.Repositories;

public class RepositoryResponse
{
    public bool IsOk { get; set; }

    public string Message { get; set; } = String.Empty;
}
public interface IRepository<TEntity> where TEntity: IEntity
{

    Task<TEntity> Find(Guid id);
    Task<IEnumerable<TEntity>> List(int page = 0, int pageSize = 0);
    IQueryable<TEntity> Query();
    Task<RepositoryResponse> Create(TEntity entity);
    Task<RepositoryResponse> Update(TEntity entity);
    Task<RepositoryResponse> Delete(Guid id);
    Task<RepositoryResponse> Delete(TEntity entity);
}
