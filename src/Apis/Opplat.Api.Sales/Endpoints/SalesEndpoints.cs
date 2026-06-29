using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Sales.Common;
using Opplat.Application.Features.Sales.CostTabs;
using Opplat.Application.Features.Sales.ProductTags;
using Opplat.Application.Features.Sales.Sales;
using Opplat.Application.Features.Sales.Toppings;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Api.Sales.Endpoints;

public static class SalesEndpoints
{
    public static void MapSalesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var sales = endpoints.MapGroup("/sales").WithTags("Sales");

        sales.MapGet(string.Empty, async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListSalesQuery());
            return Results.Ok(result);
        }).RequireAuthorization();

        MapToppings(sales);
        MapProductTags(sales);
        MapCostTabs(sales);
    }

    private static void MapToppings(RouteGroupBuilder sales)
    {
        sales.MapGet("/toppings/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetToppingQuery(id));
            return Results.Ok(result);
        });

        sales.MapGet("/toppings", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListToppingsQuery());
            return Results.Ok(result);
        });

        sales.MapPost("/toppings", async (Topping topping, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateToppingCommand(topping, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        sales.MapPut("/toppings", async (Topping topping, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpdateToppingCommand(topping, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        sales.MapDelete("/toppings/{id}", async (string id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new DeleteToppingCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });
    }

    private static void MapProductTags(RouteGroupBuilder sales)
    {
        sales.MapGet("/producttags/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProductTagQuery(id));
            return Results.Ok(result);
        });

        sales.MapGet("/producttags", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListProductTagsQuery());
            return Results.Ok(result);
        });

        sales.MapPost("/producttags", async (ProductTag productTag, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateProductTagCommand(productTag, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        sales.MapPut("/producttags", async (ProductTag productTag, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpdateProductTagCommand(productTag, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        sales.MapDelete("/producttags/{id}", async (string id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new DeleteProductTagCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });
    }

    private static void MapCostTabs(RouteGroupBuilder sales)
    {
        sales.MapGet("/costtabs/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCostTabQuery(id));
            return Results.Ok(result);
        });

        sales.MapGet("/costtabs", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListCostTabsQuery());
            return Results.Ok(result);
        });

        sales.MapPost("/costtabs", async (CostTab costTab, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateCostTabCommand(costTab, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        sales.MapPut("/costtabs", async (CostTab costTab, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpdateCostTabCommand(costTab, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        sales.MapDelete("/costtabs/{id}", async (string id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new DeleteCostTabCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });
    }

    private static ResponseDto BuildResponse(SalesCommandResult result)
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