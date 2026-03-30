using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Opplat.MainApp.Middleware;

namespace Opplat.MainApp.Extensions;

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
