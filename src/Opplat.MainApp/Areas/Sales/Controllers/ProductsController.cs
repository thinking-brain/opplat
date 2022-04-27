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

// [Authorize]
[Area("Sales")]
[Route("[area]/[controller]/")]
public class ProductsController : ControllerBase
{
    private IProductService _prodService;

    public ProductsController(IProductService prodService)
    {
        _prodService = prodService;
    }

    [HttpGet()]
    public async Task<IEnumerable<Product>> List()
    {
        var result = await _prodService.List(null,null);
        if(result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<Product>();
    }

    [HttpPost]
    public async Task<IActionResult> Post(Product prod)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Create(prod, user);
        if(result.Status == ServiceStatus.Ok)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }

    [HttpPut]
    public async Task<IActionResult> Put(Product prod)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Update(prod, user);
        if(result.Status == ServiceStatus.Ok)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Delete(id, user);
        if(result.Status == ServiceStatus.Ok)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }
}
