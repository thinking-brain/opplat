using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Dtos;
using Opplat.Application.Features.Invoicing.Common;
using Opplat.Application.Features.Invoicing.Invoices;
using Opplat.Application.Features.Invoicing.Settings;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Api.Invoicing.Endpoints;

public static class InvoicingEndpoints
{
    public static void MapInvoicingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var invoices = endpoints.MapGroup("/invoices").WithTags("Invoicing");
        invoices.MapGet(string.Empty, async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new ListInvoicesQuery());
            return Results.Ok(result);
        }).RequireAuthorization();

        invoices.MapGet("/{id}", async (string id, [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetInvoiceQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).RequireAuthorization();

        invoices.MapPost(string.Empty, async (Invoice invoice, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CreateInvoiceCommand(invoice, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        }).RequireAuthorization();

        invoices.MapPost("/{id}/issue", async (string id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new IssueInvoiceCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        }).RequireAuthorization();

        invoices.MapPost("/{id}/cancel", async (string id, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new CancelInvoiceCommand(id, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        }).RequireAuthorization();

        invoices.MapGet("/{id}/pdf", async (string id, [FromServices] IMediator mediator) =>
        {
            var pdfBytes = await mediator.Send(new GetInvoicePdfQuery(id));
            return pdfBytes is null
                ? Results.NotFound()
                : Results.File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
        }).RequireAuthorization();

        invoices.MapPost("/{id}/send", async (string id, [FromQuery] string email, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new SendInvoiceEmailCommand(id, email, GetCurrentUser(httpContext)));
            return BuildResponse(result);
        }).RequireAuthorization();

        var settings = endpoints.MapGroup("/invoicing/settings").WithTags("Invoicing");
        settings.MapGet(string.Empty, async ([FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTenantFiscalSettingsQuery());
            return Results.Ok(result ?? new TenantFiscalSettings
            {
                LegalName = string.Empty,
                TaxId = string.Empty,
                FiscalAddress = string.Empty,
                DefaultSeries = string.Empty
            });
        }).RequireAuthorization();

        settings.MapPut(string.Empty, async (TenantFiscalSettings settingsValue, [FromServices] IMediator mediator, HttpContext httpContext) =>
        {
            var result = await mediator.Send(new UpsertTenantFiscalSettingsCommand(settingsValue));
            return BuildResponse(result);
        }).RequireAuthorization();
    }

    private static IResult BuildResponse(InvoiceCommandResult result)
    {
        var response = new ResponseDto
        {
            Status = result.Succeeded,
            Message = result.Message,
            Errors = [.. result.Errors]
        };

        return result.Succeeded ? Results.Ok(response) : Results.BadRequest(response);
    }

    private static string? GetCurrentUser(HttpContext httpContext)
        => httpContext.User?.Identity?.Name;
}