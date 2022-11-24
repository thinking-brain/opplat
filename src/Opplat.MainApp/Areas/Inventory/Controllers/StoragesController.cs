using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Services;
using Opplat.MainApp.Dtos;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Inventory.Controllers;

// [Authorize]
[Area("inventory")]
[Route("[area]/[controller]/")]
public class StoragesController : ControllerBase
{
    private readonly IStorageService _service;

    public StoragesController(IStorageService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<Storage> Get(string id)
    {
        var result = await _service.Get(id);
        if(result.Status == ServiceStatus.Ok)
        {
            return result.Value;
        }
        return new Storage();
    }

    [HttpGet()]
    public async Task<IEnumerable<Storage>> List()
    {
        var result = await _service.List();
        if(result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<Storage>();
    }

    [HttpPost]
    public async Task<ResponseDto> Post([FromBody]Storage storage)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Create(storage, user);
        if(result.Status == ServiceStatus.Ok)
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
    public async Task<ResponseDto> Put([FromBody]Storage prod)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Update(prod, user);
        if(result.Status == ServiceStatus.Ok)
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

    [HttpDelete("{id}")]
    public async Task<ResponseDto> Delete(string id)
    {
        var user = User?.Identity?.Name;
        var result = await _service.Delete(id, user);
        if(result.Status == ServiceStatus.Ok)
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
