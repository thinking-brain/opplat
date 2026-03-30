using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Services.Sales;

public interface ISalesService : IService<Sale, string>
{
    Task<List<Sale>> GetSalesByDate(DateTime date);
    Task<ServiceResponse<Sale>> CreateNewSaleInDate(Sale sale, DateTime date, string user);

}

public class SalesService(ISalesRepository repo, ILogger<Sale> logger) : BaseService<Sale, string>(repo, logger), ISalesService
{
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

    public override async Task<ServiceResponse<Sale>> Get(string id)
    {
        var entity = await _repo.Find(new Guid(id));
        var result = new ServiceResponse<Sale>
        {
            Status = entity != null ? ServiceStatus.Ok : ServiceStatus.Error,
            Message = entity != null ? "Venta encontrada" : "Venta no encontrada",
            Value = entity,
        };
        return result;
    }
}
