using Microsoft.AspNetCore.Mvc;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Services;
using Opplat.MainApp.Dtos;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Sales.Controllers;

// [Authorize]
[Area("sales")]
[Route("[area]/[controller]/")]
public class ProductTagsController : ControllerBase
{
    private readonly IProductTagService _service;

    public ProductTagsController(IProductTagService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ProductTag> Get(string id)
    {
        var result = await _service.Get(id);
        if (result.Status == ServiceStatus.Ok)
        {
            return result.Value;
        }
        return new ProductTag();
    }

    [HttpGet()]
    public async Task<IEnumerable<ProductTag>> List()
    {
        var result = await _service.List();
        if (result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<ProductTag>();
    }

    [HttpPost]
    public async Task<ResponseDto> Post([FromBody] ProductTag productTag)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Create(productTag, user);
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
    public async Task<ResponseDto> Put([FromBody] ProductTag productTag)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Update(productTag, user);
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
    public async Task<ResponseDto> Delete(string id)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Delete(id, user);
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
