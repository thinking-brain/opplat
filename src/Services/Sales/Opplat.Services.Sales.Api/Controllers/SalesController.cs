// SalesController has been replaced by Endpoints/SalesEndpoints.cs (Minimal API + MediatR).
// This file is kept for reference only and is not mapped by the host.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Services;
using Opplat.Shared.Services;

namespace Opplat.Services.Sales.Api.Controllers;

// [Authorize]
// [Route("[controller]")]
public class SalesController_Archived : ControllerBase
{
    private ISalesService _saleService;

    public SalesController_Archived(ISalesService saleService)
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

