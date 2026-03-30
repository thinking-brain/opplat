using Microsoft.Extensions.Logging;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Application.Services.Sales;

public interface IToppingService : IService<Topping, string>
{

}

public class ToppingService(IToppingRepository repo, ILogger<Topping> logger) : BaseService<Topping, string>(repo, logger), IToppingService
{
}
