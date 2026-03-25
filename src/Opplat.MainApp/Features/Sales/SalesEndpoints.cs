using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opplat.MainApp.Dtos;
using Opplat.Application.Sales.Common;
using Opplat.Application.Sales.CostTabs;
using Opplat.Application.Sales.Products;
using Opplat.Application.Sales.ProductTags;
using Opplat.Application.Sales.Sales;
using Opplat.Application.Sales.Toppings;
using Opplat.Modules.Sales.Domain.Entities;

namespace Opplat.MainApp.Features.Sales;

public static class SalesEndpoints
{
    public static void MapSalesEndpoints(this WebApplication app)
    {
        MapSalesGroup(app.MapGroup("/sales"));
        MapSalesGroup(app.MapGroup("/{__tenant__}/sales"));
    }

    private static void MapSalesGroup(RouteGroupBuilder sales)
    {
        sales.MapGet(string.Empty, async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListSalesQuery());
            return Results.Ok(result);
        }).RequireAuthorization();

        MapProducts(sales);
        MapToppings(sales);
        MapProductTags(sales);
        MapCostTabs(sales);
    }

    private static void MapProducts(RouteGroupBuilder sales)
    {
        sales.MapGet("/products", async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListProductsQuery());
            return Results.Ok(result);
        });

        sales.MapPost("/products", async (ProductForSale product, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateProductCommand(product, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        sales.MapPut("/products", async (ProductForSale product, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpdateProductCommand(product, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });

        sales.MapDelete("/products/{id}", async (string id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new DeleteProductCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        });
    }

    private static void MapToppings(RouteGroupBuilder sales)
    {
        sales.MapGet("/toppings/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetToppingQuery(id));
            return Results.Ok(result ?? new Topping());
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
            return Results.Ok(result ?? new ProductTag());
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
            return Results.Ok(result ?? new CostTab());
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
            Errors = result.Errors.ToList()
        };
    }

    private static string? GetCurrentUser(HttpContext httpContext)
    {
        return httpContext.User?.Identity?.Name;
    }
}

