using Microsoft.AspNetCore.Mvc;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Services;
using Opplat.MainApp.Dtos;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Inventory.Controllers;

// [Authorize]
[Area("inventory")]
[Route("[area]/[controller]/")]
public class ProductMovementsController : ControllerBase
{
    private readonly IProductMovementService _service;

    public ProductMovementsController(IProductMovementService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IEnumerable<ProductMovement>> List(string id)
    {
        var result = await _service.GetByStorage(id);
        if (result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<ProductMovement>();
    }

    [HttpGet()]
    public async Task<IEnumerable<ProductMovement>> List()
    {
        var result = await _service.List();
        if (result.Status == ServiceStatus.Ok)
        {
            return result.List;
        }
        return new List<ProductMovement>();
    }

    [HttpPost]
    public async Task<ResponseDto> Post([FromBody] ProductMovementDto prod)
    {
        var user = User?.Identity?.Name;
        var result = await _service
            .CreateMovement(prod.ProductId,prod.StorageId, prod.Quantity,prod.Unit,prod.Date,prod.Type,user,prod.Observations);
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
