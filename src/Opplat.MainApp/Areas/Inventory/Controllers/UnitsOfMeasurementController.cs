using Microsoft.AspNetCore.Mvc;
using Opplat.Domain.Inventory.Entities;
using Opplat.Domain.Inventory.Services;
using Opplat.MainApp.Dtos;
using Opplat.Shared.Entities;
using Opplat.Shared.Helpers;
using Opplat.Shared.Services;

namespace Opplat.MainApp.Areas.Inventory.Controllers;

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
