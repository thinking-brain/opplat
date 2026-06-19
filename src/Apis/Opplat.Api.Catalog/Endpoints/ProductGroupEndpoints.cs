using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Inventory.Common;
using Opplat.Application.Features.Inventory.ProductGroups;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Api.Catalog.Endpoints;

public static class ProductGroupEndpoints
{
    public const string productGroupRoute = "/productgroups";

    public static void MapProductGroups(RouteGroupBuilder catalog)
    {
        catalog.MapGet($"{productGroupRoute}/{{id:int}}", GetProductGroupByIdAsync)
            .WithName("GetProductGroupById")
            .Produces<ProductGroup>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        catalog.MapGet(productGroupRoute, GetProductGroupsAsync)
            .WithName("GetProductGroups")
            .Produces<IReadOnlyList<ProductGroup>>(StatusCodes.Status200OK);

        catalog.MapPost(productGroupRoute, CreateProductGroupAsync)
            .WithName("CreateProductGroup")
            .Produces<ResponseDto>(StatusCodes.Status200OK);

        catalog.MapPut(productGroupRoute, UpdateProductGroupAsync)
            .WithName("UpdateProductGroup")
            .Produces<ResponseDto>(StatusCodes.Status200OK);

        catalog.MapDelete($"{productGroupRoute}/{{id:int}}", DeleteProductGroupAsync)
            .WithName("DeleteProductGroup")
            .Produces<ResponseDto>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetProductGroupsAsync([FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new ListProductGroupsQuery());
        return Results.Ok(result);
    }

    private static async Task<ProductGroup?> GetProductGroupByIdAsync(int id, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetProductGroupQuery(id));
        return result;
    }

    private static async Task<ResponseDto> CreateProductGroupAsync(ProductGroup productGroup, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new CreateProductGroupCommand(productGroup, GetCurrentUser(httpContext)));
        return BuildResponse(result);
    }

    private static async Task<ResponseDto> UpdateProductGroupAsync(ProductGroup productGroup, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new UpdateProductGroupCommand(productGroup, GetCurrentUser(httpContext)));
        return BuildResponse(result);
    }

    private static async Task<ResponseDto> DeleteProductGroupAsync(int id, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new DeleteProductGroupCommand(id, GetCurrentUser(httpContext)));
        return BuildResponse(result);
    }

    private static ResponseDto BuildResponse(InventoryCommandResult result)
    {
        return new ResponseDto
        {
            Status = result.Succeeded,
            Message = result.Message,
            Errors = [.. result.Errors]
        };
    }

    private static string? GetCurrentUser(HttpContext httpContext)
    {
        return httpContext.User?.Identity?.Name;
    }
}