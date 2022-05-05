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
public class ToppingsController : ControllerBase
{
    private IToppingService _service;

    public ToppingsController(IToppingService service)
    {
        _service = service;
    }

    [HttpGet()]
    public async Task<IEnumerable<Topping>> List()
    {
        var result = await _service.List();
        if(result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<Topping>();
    }

    [HttpGet("{id}")]
    public async Task<Topping?> Get(string id)
    {
        var result = await _service.Get(id);
        if(result.Status == ServiceStatus.Ok)
        {
            return result.Value;
        }
        return null;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Topping topping)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Create(topping, user);
        if(result.Status == ServiceStatus.Ok)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }

    [HttpPut]
    public async Task<IActionResult> Put(Topping topping)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Update(topping, user);
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
        var result = await _service.Delete(id, user);
        if(result.Status == ServiceStatus.Ok)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }
}
