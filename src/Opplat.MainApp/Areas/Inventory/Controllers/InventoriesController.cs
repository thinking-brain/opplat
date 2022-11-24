using Microsoft.AspNetCore.Mvc;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Services;
using Opplat.MainApp.Dtos;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Inventory.Controllers;

// [Authorize]
[Area("inventory")]
[Route("[area]/[controller]/")]
public class InventoriesController : ControllerBase
{
    private readonly IInventoryService _service;

    public InventoriesController(IInventoryService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IEnumerable<InventoryDto>> List(string id)
    {
        var result = await _service.GetByStorage(new Guid(id));
        if (result.Status == ServiceStatus.Ok)
        {
            return result.List.Select(p => new InventoryDto{
                ProductId = p.ProductId,
                Product = p.Product.Name,
                Quantity = p.Quantity,
                Unit = p.Product.Unit
            });
        }
        return new List<InventoryDto>();
    }
}
