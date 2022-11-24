using Microsoft.AspNetCore.Mvc;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Services;
using Opplat.MainApp.Dtos;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Sales.Controllers;

// [Authorize]
[Area("sales")]
[Route("[area]/[controller]/")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _prodService;

    public ProductsController(IProductService prodService)
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
