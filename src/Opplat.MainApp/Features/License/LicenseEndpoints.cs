using MediatR;
using Opplat.MainApp.Features.License.Commands;
using Opplat.MainApp.Features.License.Queries;

namespace Opplat.MainApp.Features.License;

public static class LicenseEndpoints
{
    public static void MapLicenseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/admin/licencia").WithTags("License");

        group.MapGet("/",
            async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetLicenseQuery());
                return result.Success
                    ? Results.Ok(result.Licencia)
                    : Results.BadRequest(result.ErrorMessage);
            })
            .WithSummary("Obtener licencia activa");

        group.MapPost("/",
            async (IFormFile licence, IMediator mediator) =>
            {
                var result = await mediator.Send(new AddLicenseCommand(licence));
                return result.Success
                    ? Results.Ok(result.Licencia)
                    : Results.BadRequest(result.ErrorMessage);
            })
            .DisableAntiforgery()
            .WithSummary("Subir licencia");

        group.MapDelete("/",
            async (IMediator mediator) =>
            {
                var ok = await mediator.Send(new DeleteLicenseCommand());
                return ok
                    ? Results.Ok("Licencia borrada correctamente.")
                    : Results.BadRequest("Error eliminando la licencia, contacte al administrador.");
            })
            .WithSummary("Eliminar licencia");
    }
}
