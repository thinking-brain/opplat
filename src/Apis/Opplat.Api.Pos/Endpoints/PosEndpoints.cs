using System.Text;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Features.Sales.Pos;
using Opplat.Domain.Entities.Sales;

namespace Opplat.Api.Pos.Endpoints;

public sealed record CreateRegisterRequest(string Code, string Name, string InvoiceSeries, Guid? StorageId);

public sealed record CreateTerminalRequest(Guid? RegisterId, string Provider, string ProviderTerminalId, string DisplayName);

public sealed record OpenShiftRequest(decimal OpeningFloat);

public sealed record CloseShiftRequest(decimal CountedCash);

public sealed record CashMovementRequest(PosCashMovementType Type, decimal Amount, string Reason);

public static class PosEndpoints
{
    public static void MapPosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var pos = endpoints.MapGroup("/pos").WithTags("POS");

        MapRegistersAndTerminals(pos);
        MapShifts(pos);
        MapSales(pos);
        MapPayments(pos);

        // Provider callbacks are authenticated by their own signature, not a user token.
        pos.MapPost("/webhooks/{provider}", async (string provider, HttpRequest httpRequest, [FromServices] IMediator mediator) =>
        {
            using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            var headers = httpRequest.Headers.ToDictionary(h => h.Key, h => h.Value.ToString(), StringComparer.OrdinalIgnoreCase);
            var result = await mediator.Send(new ApplyTerminalWebhookCommand(provider, body, headers));
            return ToResult(result);
        }).AllowAnonymous();
    }

    private static void MapRegistersAndTerminals(RouteGroupBuilder pos)
    {
        pos.MapGet("/registers", async ([FromServices] IMediator mediator)
            => Results.Ok(await mediator.Send(new ListRegistersQuery()))).RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/registers", async (CreateRegisterRequest request, [FromServices] IMediator mediator, HttpContext http) =>
            ToResult(await mediator.Send(new CreateRegisterCommand(request.Code, request.Name, request.InvoiceSeries, request.StorageId, User(http)))))
            .RequireAuthorization(AuthRoles.PosManagerPolicy);

        pos.MapGet("/terminals", async ([FromServices] IMediator mediator)
            => Results.Ok(await mediator.Send(new ListTerminalsQuery()))).RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapGet("/payment-providers", async ([FromServices] IMediator mediator)
            => Results.Ok(await mediator.Send(new ListPaymentProvidersQuery()))).RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/terminals", async (CreateTerminalRequest request, [FromServices] IMediator mediator, HttpContext http) =>
            ToResult(await mediator.Send(new CreateTerminalCommand(request.RegisterId, request.Provider, request.ProviderTerminalId, request.DisplayName, User(http)))))
            .RequireAuthorization(AuthRoles.PosManagerPolicy);

        pos.MapDelete("/terminals/{id:guid}", async (Guid id, [FromServices] IMediator mediator)
            => ToResult(await mediator.Send(new DeactivateTerminalCommand(id)))).RequireAuthorization(AuthRoles.PosManagerPolicy);
    }

    private static void MapShifts(RouteGroupBuilder pos)
    {
        pos.MapGet("/registers/{registerId:guid}/shift", async (Guid registerId, [FromServices] IMediator mediator) =>
        {
            var shift = await mediator.Send(new GetCurrentShiftQuery(registerId));
            return shift is null ? Results.NotFound() : Results.Ok(shift);
        }).RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/registers/{registerId:guid}/shifts", async (Guid registerId, OpenShiftRequest request, [FromServices] IMediator mediator, HttpContext http)
            => ToResult(await mediator.Send(new OpenShiftCommand(registerId, request.OpeningFloat, User(http)))))
            .RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/shifts/{id:guid}/close", async (Guid id, CloseShiftRequest request, [FromServices] IMediator mediator, HttpContext http)
            => ToResult(await mediator.Send(new CloseShiftCommand(id, request.CountedCash, User(http)))))
            .RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/shifts/{id:guid}/cash-movements", async (Guid id, CashMovementRequest request, [FromServices] IMediator mediator, HttpContext http)
            => ToResult(await mediator.Send(new AddCashMovementCommand(id, request.Type, request.Amount, request.Reason, User(http)))))
            .RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapGet("/shifts/{id:guid}/report", async (Guid id, [FromServices] IMediator mediator) =>
        {
            var report = await mediator.Send(new GetShiftReportQuery(id));
            return report is null ? Results.NotFound() : Results.Ok(report);
        }).RequireAuthorization(AuthRoles.PosOperatorPolicy);
    }

    private static void MapSales(RouteGroupBuilder pos)
    {
        pos.MapPost("/sales", async (CreateSaleRequest request, [FromServices] IMediator mediator, HttpContext http) =>
            ToResult(await mediator.Send(new CreateSaleCommand(request, User(http)))))
            .RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapGet("/sales", async ([FromQuery] Guid? shiftId, [FromQuery] int? take, [FromServices] IMediator mediator)
            => Results.Ok(await mediator.Send(new ListPosSalesQuery(shiftId, take ?? 50)))).RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapGet("/sales/{id:guid}", async (Guid id, [FromServices] IMediator mediator) =>
        {
            var sale = await mediator.Send(new GetPosSaleQuery(id));
            return sale is null ? Results.NotFound() : Results.Ok(sale);
        }).RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/sales/{id:guid}/payments", async (
            Guid id,
            AddPaymentRequest request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            [FromServices] IMediator mediator,
            HttpContext http) =>
        {
            var merged = string.IsNullOrWhiteSpace(request.IdempotencyKey) ? request with { IdempotencyKey = idempotencyKey } : request;
            return ToResult(await mediator.Send(new AddPaymentCommand(id, merged, User(http))));
        }).RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/sales/{id:guid}/complete", async (Guid id, CompleteSaleRequest? request, [FromServices] IMediator mediator, HttpContext http)
            => ToResult(await mediator.Send(new CompleteSaleCommand(id, request, User(http)))))
            .RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/sales/{id:guid}/void", async (Guid id, [FromServices] IMediator mediator, HttpContext http)
            => ToResult(await mediator.Send(new VoidSaleCommand(id, User(http)))))
            .RequireAuthorization(AuthRoles.PosOperatorPolicy);
    }

    private static void MapPayments(RouteGroupBuilder pos)
    {
        pos.MapGet("/payments/{id:guid}", async (Guid id, [FromServices] IMediator mediator)
            => ToResult(await mediator.Send(new GetPaymentQuery(id)))).RequireAuthorization(AuthRoles.PosOperatorPolicy);

        pos.MapPost("/payments/{id:guid}/cancel", async (Guid id, [FromServices] IMediator mediator)
            => ToResult(await mediator.Send(new CancelPaymentCommand(id)))).RequireAuthorization(AuthRoles.PosOperatorPolicy);
    }

    private static IResult ToResult<T>(PosResult<T> result)
    {
        if (result.Succeeded)
        {
            return Results.Ok(result.Data);
        }

        return result.Error switch
        {
            PosErrorKind.NotFound => Results.Problem(result.Message, statusCode: StatusCodes.Status404NotFound),
            PosErrorKind.Conflict => Results.Problem(result.Message, statusCode: StatusCodes.Status409Conflict),
            _ => Results.Problem(result.Message, statusCode: StatusCodes.Status400BadRequest)
        };
    }

    private static string? User(HttpContext httpContext) => httpContext.User.Identity?.Name;
}
