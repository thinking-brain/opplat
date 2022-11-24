using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Services;
using Opplat.MainApp.Dtos;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Inventory.Controllers;

// [Authorize]
[Area("inventory")]
[Route("[area]/[controller]/")]
public class ProductClassificationsController : ControllerBase
{
    private readonly IProductClassificationService _prodService;

    public ProductClassificationsController(IProductClassificationService prodService)
    {
        _prodService = prodService;
    }

    [HttpGet("{id}")]
    public async Task<ProductClassification> Get(int id)
    {
        var result = await _prodService.Get(id);
        if (result.Status == ServiceStatus.Ok)
        {
            return result.Value;
        }
        return new ProductClassification();
    }

    [HttpGet()]
    public async Task<IEnumerable<ProductClassification>> List()
    {
        var result = await _prodService.List();
        if (result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<ProductClassification>();
    }

    [HttpPost]
    public async Task<ResponseDto> Post([FromBody] ProductClassification prod)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Create(prod, user);
        if (result.Status == ServiceStatus.Ok)
        {
            return new ResponseDto
            {
                Status = true,
                Message = result.Message,
                Errors = new List<string>() { }
            };
        }
        return new ResponseDto
        {
            Status = false,
            Message = result.Message,
            Errors = new List<string>() { result.Message }
        };
    }

    [HttpPut]
    public async Task<ResponseDto> Put([FromBody] ProductClassification prod)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Update(prod, user);
        if (result.Status == ServiceStatus.Ok)
        {
            return new ResponseDto
            {
                Status = true,
                Message = result.Message,
                Errors = new List<string>() { }
            };
        }
        return new ResponseDto
        {
            Status = false,
            Message = result.Message,
            Errors = new List<string>() { result.Message }
        };
    }

    [HttpDelete("{id}")]
    public async Task<ResponseDto> Delete(int id)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Delete(id, user);
        if (result.Status == ServiceStatus.Ok)
        {
            return new ResponseDto
            {
                Status = true,
                Message = result.Message,
                Errors = new List<string>() { }
            };
        }
        return new ResponseDto
        {
            Status = false,
            Message = result.Message,
            Errors = new List<string>() { result.Message }
        };
    }
}
