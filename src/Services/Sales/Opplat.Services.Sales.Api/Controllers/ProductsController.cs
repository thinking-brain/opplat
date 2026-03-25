// ProductsController has been replaced by Endpoints/SalesEndpoints.cs (Minimal API + MediatR).
// This file is kept for reference only and is not mapped by the host.

using Microsoft.AspNetCore.Mvc;
using Opplat.Modules.Sales.Domain.Entities;
using Opplat.Modules.Sales.Domain.Services;
using Opplat.Microservices.Shared.Dtos;
using Opplat.Shared.Services;

namespace Opplat.Services.Sales.Api.Controllers;

// [Authorize]
// [Area("sales")]
// [Route("[area]/[controller]/")]
public class ProductsController_Archived : ControllerBase
{
    private readonly IProductService _prodService;

    public ProductsController_Archived(IProductService prodService)
    {
        _prodService = prodService;
    }

    [HttpGet()]
    public async Task<IEnumerable<ProductForSale>> List()
    {
        var result = await _prodService.List();
        if(result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<ProductForSale>();
    }

    [HttpPost]
    public async Task<ResponseDto> Post([FromBody]ProductForSale prod)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Create(prod, user);
        if (result.Status == ServiceStatus.Ok)
        {
            return new ResponseDto
            {
                Status = true,
                Message = result.Message,
                Errors = new List<string>() {  }
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
    public async Task<ResponseDto> Put([FromBody]ProductForSale prod)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Update(prod, user);
        if (result.Status == ServiceStatus.Ok)
        {
            return new ResponseDto
            {
                Status = true,
                Message = result.Message,
                Errors = new List<string>() {  }
            };
        }
        return new ResponseDto
        {
            Status = false,
            Message = result.Message,
            Errors = new List<string>() { result.Message }
        };
    }

    [HttpDelete]
    public async Task<ResponseDto> Delete(string id)
    {
        var user = User?.Identity?.Name;
        var result = await _prodService.Delete(id, user);
        if (result.Status == ServiceStatus.Ok)
        {
            return new ResponseDto
            {
                Status = true,
                Message = result.Message,
                Errors = new List<string>() {  }
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

