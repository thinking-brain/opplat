using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Services;
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
