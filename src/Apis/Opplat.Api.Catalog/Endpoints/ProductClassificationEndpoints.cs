using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Inventory.Common;
using Opplat.Application.Features.Inventory.ProductClassifications;
using Opplat.Domain.Entities.Inventory;

namespace Opplat.Api.Catalog.Endpoints;

public static class ProductClassificationEndpoints
{
    public const string productClassificationRoute = "/product-classifications";

    public static void MapProductClassifications(RouteGroupBuilder catalog)
    {
        catalog.MapGet($"{productClassificationRoute}/{{id:int}}", GetProductClassificationByIdAsync)
            .WithName("GetProductClassificationById")
            .Produces<ProductClassification>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        catalog.MapGet(productClassificationRoute, GetProductClassificationsAsync)
            .WithName("GetProductClassifications")
            .Produces<IReadOnlyList<ProductClassification>>(StatusCodes.Status200OK);

        catalog.MapPost(productClassificationRoute, CreateProductClassificationAsync)
            .WithName("CreateProductClassification")
            .Produces<ResponseDto>(StatusCodes.Status200OK);

        catalog.MapPut(productClassificationRoute, UpdateProductClassificationAsync)
            .WithName("UpdateProductClassification")
            .Produces<ResponseDto>(StatusCodes.Status200OK);

        catalog.MapDelete($"{productClassificationRoute}/{{id:int}}", DeleteProductClassificationAsync)
            .WithName("DeleteProductClassification")
            .Produces<ResponseDto>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetProductClassificationsAsync([FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new ListProductClassificationsQuery());
        return Results.Ok(result);
    }

    private static async Task<ProductClassification?> GetProductClassificationByIdAsync(int id, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetProductClassificationQuery(id));
        return result;
    }

    private static async Task<ResponseDto> CreateProductClassificationAsync(ProductClassification classification, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new CreateProductClassificationCommand(classification, GetCurrentUser(httpContext)));
        return BuildResponse(result);
    }

    private static async Task<ResponseDto> UpdateProductClassificationAsync(ProductClassification classification, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new UpdateProductClassificationCommand(classification, GetCurrentUser(httpContext)));
        return BuildResponse(result);
    }

    private static async Task<ResponseDto> DeleteProductClassificationAsync(int id, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new DeleteProductClassificationCommand(id, GetCurrentUser(httpContext)));
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