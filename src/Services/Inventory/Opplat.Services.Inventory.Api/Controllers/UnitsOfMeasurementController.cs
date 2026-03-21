using Microsoft.AspNetCore.Mvc;
using Opplat.Modules.Inventory.Domain.Entities;
using Opplat.Modules.Inventory.Domain.Services;
using Opplat.Microservices.Shared.Dtos;
using Opplat.Shared.Entities;
using Opplat.Shared.Helpers;
using Opplat.Shared.Services;

namespace Opplat.Services.Inventory.Api.Controllers;

// [Authorize]
[Area("inventory")]
[Route("[area]/[controller]/")]
public class UnitsOfMeasurementController : ControllerBase
{
    public UnitsOfMeasurementController()
    {
        
    }

    [HttpGet()]
    public IEnumerable<UnitOfMeasurement> List()
    {
        return UnitOfMeasurementHelper.GetUnits();
    }
}

