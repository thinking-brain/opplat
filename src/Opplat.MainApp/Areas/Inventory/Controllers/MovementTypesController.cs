using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opplat.Domain.Inventory.Dtos;
using Opplat.Domain.Inventory.Services;

namespace Opplat.MainApp.Areas.Inventory.Controllers;

[Authorize]
[Area("inventory")]
[Route("[area]/[controller]/")]
public class MovementTypesController : ControllerBase
{
    private readonly IMovementTypeService _service;

    public MovementTypesController(IMovementTypeService service)
    {
        _service = service;
    }

    [HttpGet()]
    public IEnumerable<MovementTypeDto> List()
    {
        var result = _service.List();
        return result;
    }

}
