using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Opplat.Microservices.Shared.Middleware;

namespace Opplat.Microservices.Shared.Extensions;

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
        app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/docs/v1/docs.json", "Opplat Service v1");
            c.RoutePrefix = "docs";
        });
        app.UseAuthentication();
        app.UseMiddleware<TenantValidationMiddleware>();
        app.UseAuthorization();
        app.UseCors("CorsPolicy");

        return app;
    }
}
