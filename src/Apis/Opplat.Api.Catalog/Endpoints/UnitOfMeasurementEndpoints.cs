using Opplat.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Features.Inventory.UnitsOfMeasurement;
using Opplat.Domain.Entities;

namespace Opplat.Api.Catalog.Endpoints;

public static class UnitOfMeasurementEndpoints
{
    public const string unitOfMeasurementRoute = "/units-of-measurement";

    public static void MapUnitsOfMeasurement(RouteGroupBuilder catalog)
    {
        catalog.MapGet(unitOfMeasurementRoute, GetUnitsOfMeasurementAsync)
            .WithName("GetUnitsOfMeasurement")
            .Produces<IReadOnlyList<UnitOfMeasurement>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetUnitsOfMeasurementAsync([FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new ListUnitsOfMeasurementQuery());
        return Results.Ok(result);
    }
}