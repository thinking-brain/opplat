using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Inventory.Common;
using Opplat.Application.Features.Inventory.Inventories;
using Opplat.Application.Features.Inventory.MovementTypes;
using Opplat.Application.Features.Inventory.ProductMovements;
using Opplat.Application.Features.Inventory.Storages;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Api.Inventory.Endpoints;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var inventory = endpoints.MapGroup("/inventory");
        MapStorages(inventory);
        MapMovementTypes(inventory);
        MapInventories(inventory);
        MapProductMovements(inventory);
    }

    private static void MapStorages(RouteGroupBuilder inventory)
    {
        inventory.MapGet("/storages/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetStorageQuery(id));
            return Results.Ok(result ?? new Storage());
        });

        inventory.MapGet("/storages", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListStoragesQuery());
            return Results.Ok(result);
        });

        inventory.MapPost("/storages", async (Storage storage, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateStorageCommand(storage, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        inventory.MapPut("/storages", async (Storage storage, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpdateStorageCommand(storage, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        inventory.MapDelete("/storages/{id}", async (string id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new DeleteStorageCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });
    }

    private static void MapMovementTypes(RouteGroupBuilder inventory)
    {
        inventory.MapGet("/movementtypes", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListMovementTypesQuery());
            return Results.Ok(result);
        }).RequireAuthorization();
    }

    private static void MapInventories(RouteGroupBuilder inventory)
    {
        inventory.MapGet("/inventories/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetInventoriesByStorageQuery(id));
            var dtos = result.Select(item => new InventoryDto
            {
                ProductId = item.ProductId,
                Product = item.Product.Name,
                Quantity = item.Quantity,
                Unit = item.Product.Unit
            });

            return Results.Ok(dtos);
        });
    }

    private static void MapProductMovements(RouteGroupBuilder inventory)
    {
        inventory.MapGet("/productmovements/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductMovementsByStorageQuery(id));
            return Results.Ok(result);
        });

        inventory.MapGet("/productmovements", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListProductMovementsQuery());
            return Results.Ok(result);
        });

        inventory.MapPost("/productmovements", async (ProductMovementDto movement, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateProductMovementCommand(
                movement.Date,
                movement.ProductId,
                movement.StorageId,
                movement.Quantity,
                movement.Unit,
                movement.Type,
                GetCurrentUser(httpContext),
                movement.Observations));

            return BuildResponse(result);
        });
    }

    private static IResult BuildResponse(InventoryCommandResult result)
    {
        var response = new ResponseDto
        {
            Status = result.Succeeded,
            Message = result.Message,
            Errors = [.. result.Errors]
        };
        return result.Succeeded ? Results.Ok(response) : Results.BadRequest(response);
    }

    private static string? GetCurrentUser(HttpContext httpContext) =>
        httpContext.User?.Identity?.Name;
}
