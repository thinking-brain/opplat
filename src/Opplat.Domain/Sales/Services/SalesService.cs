using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Repositories;
using Opplat.Shared.Services;

namespace Opplat.Domain.Sales.Services;

public interface ISalesService : IService<Sale>
{
    Task<List<Sale>> GetSalesByDate(DateTime date);
    Task<ServiceResponse<Sale>> CreateNewSaleInDate(Sale sale, DateTime date, string user);

}

public class SalesService : BaseService<Sale> ,ISalesService
{
    public SalesService(ISalesRepository repo): base (repo)
    {
    }

    public async Task<List<Sale>> GetSalesByDate(DateTime date)
    {
        var query = _repo.Query();
        var sales = query.Where(q => q.Date == date).ToList();
        return sales;
    }

    public async Task<ServiceResponse<Sale>> CreateNewSaleInDate(Sale sale, DateTime date, string user)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<Sale>> Get(object id)
    {
        var entity = await _repo.Find(id);
        var result = new ServiceResponse<Sale>
        {
            Status = entity != null ? ServiceStatus.Ok : ServiceStatus.Error,
            Message = entity != null ? "Venta encontrada" : "Venta no encontrada",
            Value = entity,
        };
        return result;
    }
}
