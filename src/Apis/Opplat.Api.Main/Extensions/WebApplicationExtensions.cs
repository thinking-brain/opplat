using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Opplat.Api.Common.Hosting;
using Opplat.Api.Main.Middleware;
using Scalar.AspNetCore;

namespace Opplat.Api.Main.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseOpplatMicroserviceHost(this WebApplication app)
    {
        app.UseOpplatAspireDevelopmentSupport();
        app.UseMultiTenant();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirectionIfConfigured();
        app.UseRouting();
        app.MapOpenApi();
        app.MapGet("/docs/", () => Results.Redirect("/docs"));
        app.MapScalarApiReference("/docs", options =>
        {
            options.Title = "Opplat Service";
            options.OpenApiRoutePattern = "/openapi/v1.json";
        });
        app.UseAuthentication();
        app.UseMiddleware<TenantValidationMiddleware>();
        app.UseAuthorization();
        app.UseCors("CorsPolicy");

        return app;
    }
}
