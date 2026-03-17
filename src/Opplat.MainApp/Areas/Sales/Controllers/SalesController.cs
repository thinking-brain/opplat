using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Services;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Sales.Controllers;

[Authorize]
public class SalesController : ControllerBase
{
    private ISalesService _saleService;

    public SalesController(ISalesService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IEnumerable<Sale>> List()
    {
        var result = await _saleService.List(null,null);
        if(result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<Sale>();
    }
}
