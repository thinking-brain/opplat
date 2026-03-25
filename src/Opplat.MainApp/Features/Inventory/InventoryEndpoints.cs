#nullable enable

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opplat.MainApp.Dtos;
using Opplat.Application.Inventory.Common;
using Opplat.Application.Inventory.Inventories;
using Opplat.Application.Inventory.MovementTypes;
using Opplat.Application.Inventory.ProductClassifications;
using Opplat.Application.Inventory.ProductGroups;
using Opplat.Application.Inventory.ProductMovements;
using Opplat.Application.Inventory.Products;
using Opplat.Application.Inventory.Storages;
using Opplat.Application.Inventory.UnitsOfMeasurement;
using Opplat.Modules.Inventory.Domain.Entities;

namespace Opplat.MainApp.Features.Inventory;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this WebApplication app)
    {
        MapInventoryGroup(app.MapGroup("/inventory"));
        MapInventoryGroup(app.MapGroup("/{__tenant__}/inventory"));
    }

    private static void MapInventoryGroup(RouteGroupBuilder inventory)
    {
        MapProducts(inventory);
        MapProductClassifications(inventory);
        MapProductGroups(inventory);
        MapStorages(inventory);
        MapUnitsOfMeasurement(inventory);
        MapMovementTypes(inventory);
        MapInventories(inventory);
        MapProductMovements(inventory);
    }

    private static void MapProducts(RouteGroupBuilder inventory)
    {
        inventory.MapGet("/products/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductQuery(id));
            return Results.Ok(result ?? new Product());
        });

        inventory.MapGet("/products", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListProductsQuery());
            return Results.Ok(result);
        });

        inventory.MapPost("/products", async (Product product, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateProductCommand(product, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        inventory.MapPut("/products", async (Product product, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpdateProductCommand(product, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        inventory.MapDelete("/products/{id}", async (string id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new DeleteProductCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });
    }

    private static void MapProductClassifications(RouteGroupBuilder inventory)
    {
        inventory.MapGet("/productclassifications/{id:int}", async (int id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductClassificationQuery(id));
            return Results.Ok(result ?? new ProductClassification());
        });

        inventory.MapGet("/productclassifications", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListProductClassificationsQuery());
            return Results.Ok(result);
        });

        inventory.MapPost("/productclassifications", async (ProductClassification classification, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateProductClassificationCommand(classification, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        inventory.MapPut("/productclassifications", async (ProductClassification classification, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpdateProductClassificationCommand(classification, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        inventory.MapDelete("/productclassifications/{id:int}", async (int id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new DeleteProductClassificationCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });
    }

    private static void MapProductGroups(RouteGroupBuilder inventory)
    {
        inventory.MapGet("/productgroups/{id:int}", async (int id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductGroupQuery(id));
            return Results.Ok(result ?? new ProductGroup());
        });

        inventory.MapGet("/productgroups", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListProductGroupsQuery());
            return Results.Ok(result);
        });

        inventory.MapPost("/productgroups", async (ProductGroup productGroup, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateProductGroupCommand(productGroup, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        inventory.MapPut("/productgroups", async (ProductGroup productGroup, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpdateProductGroupCommand(productGroup, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        inventory.MapDelete("/productgroups/{id:int}", async (int id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new DeleteProductGroupCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });
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

    private static void MapUnitsOfMeasurement(RouteGroupBuilder inventory)
    {
        inventory.MapGet("/unitsofmeasurement", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListUnitsOfMeasurementQuery());
            return Results.Ok(result);
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

    private static ResponseDto BuildResponse(InventoryCommandResult result)
    {
        return new ResponseDto
        {
            Status = result.Succeeded,
            Message = result.Message,
            Errors = result.Errors.ToList()
        };
    }

    private static string? GetCurrentUser(HttpContext httpContext)
    {
        return httpContext.User?.Identity?.Name;
    }
}

