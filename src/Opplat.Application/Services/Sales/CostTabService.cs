using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Services.Sales;

public interface ICostTabService : IService<CostTab, string>
{
    
}

public class CostTabService(ICostTabRepository repo, ILogger<CostTab> logger) : BaseService<CostTab, string>(repo, logger) ,ICostTabService
{
}
