using Opplat.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Inventory.Common;
using Opplat.Application.Features.Inventory.Products;
using Opplat.Domain.Entities.Inventory;
namespace Opplat.Api.Catalog.Endpoints;

public static class ProductEndpoints
{
    public const string productRoute = "/products";

    public static void MapProducts(RouteGroupBuilder catalog)
    {
        catalog.MapGet($"{productRoute}/{{id}}", GetProductByIdAsync)
            .WithName("GetProductById")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        catalog.MapGet(productRoute, GetProductsAsync)
            .WithName("GetProducts")
            .Produces<IReadOnlyList<Product>>(StatusCodes.Status200OK);

        catalog.MapPost(productRoute, CreateProductAsync)
            .WithName("CreateProduct")
            .Produces<ResponseDto>(StatusCodes.Status200OK);

        catalog.MapPut(productRoute, UpdateProductAsync)
            .WithName("UpdateProduct")
            .Produces<ResponseDto>(StatusCodes.Status200OK);

        catalog.MapDelete($"{productRoute}/{{id}}", DeleteProductAsync)
            .WithName("DeleteProduct")
            .Produces<ResponseDto>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetProductsAsync([FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new ListProductsQuery());
        return Results.Ok(result);
    }

    private static async Task<Product?> GetProductByIdAsync(string id, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetProductQuery(id));
        return result;
    }

    private static async Task<ResponseDto> CreateProductAsync(Product product, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new CreateProductCommand(product, GetCurrentUser(httpContext)));
        return BuildResponse(result);
    }

    private static async Task<ResponseDto> UpdateProductAsync(Product product, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new UpdateProductCommand(product, GetCurrentUser(httpContext)));
        return BuildResponse(result);
    }

    private static async Task<ResponseDto> DeleteProductAsync(string id, [FromServices] IMediator mediator, HttpContext httpContext)
    {
        var result = await mediator.Send(new DeleteProductCommand(id, GetCurrentUser(httpContext)));
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

