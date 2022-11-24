using Microsoft.AspNetCore.Mvc;
using Opplat.Domain.Sales.Entities;
using Opplat.Domain.Sales.Services;
using Opplat.MainApp.Dtos;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Sales.Controllers;

// [Authorize]
[Area("sales")]
[Route("[area]/[controller]/")]
public class ToppingsController : ControllerBase
{
    private readonly IToppingService _service;

    public ToppingsController(IToppingService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<Topping> Get(string id)
    {
        var result = await _service.Get(id);
        if (result.Status == ServiceStatus.Ok)
        {
            return result.Value;
        }
        return new Topping();
    }

    [HttpGet()]
    public async Task<IEnumerable<Topping>> List()
    {
        var result = await _service.List();
        if (result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<Topping>();
    }

    [HttpPost]
    public async Task<ResponseDto> Post([FromBody] Topping topping)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Create(topping, user);
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
    public async Task<ResponseDto> Put([FromBody] Topping topping)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Update(topping, user);
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
