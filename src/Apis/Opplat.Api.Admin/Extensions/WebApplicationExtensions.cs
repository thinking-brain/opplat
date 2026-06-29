using Opplat.Api.Admin.Endpoints;
using Opplat.Api.Common.Hosting;
using Opplat.Api.Common.Middleware;

namespace Opplat.Api.Admin.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> ConfigureAdminApp(this WebApplication app)
    {
        // await AdminPortalDataSeeder.InitializeAsync(app.Services);

        app.UseOpplatAspireDevelopmentSupport();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
            app.UseHttpsRedirectionIfConfigured();
        }

        // if (app.Environment.IsDevelopment())
        // {
        //     app.UseWhen(
        //         context => !IsAdminBffDevelopmentRequest(context.Request.Path, adminBffOptions),
        //         branch => branch.UseHttpsRedirectionIfConfigured(app.Configuration));
        // }
        // else
        // {
        //     app.UseHttpsRedirectionIfConfigured();
        // }

        app.UseCors("CorsPolicy");
        app.UseRouting();
        app.UseAuthentication();
        app.UseMiddleware<AdminBffAntiforgeryMiddleware>();
        app.UseAuthorization();

        app.MapGeneralEndpoints();
        app.MapAdminEndpoints();
        app.MapAuthEndpoints();
        return app;
    }


    // static bool IsAdminBffDevelopmentRequest(PathString path, AdminBffOptions adminBffOptions) =>
    //     path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase)
    //     || path.StartsWithSegments("/auth/bff/admin", StringComparison.OrdinalIgnoreCase)
    //     || path.StartsWithSegments(adminBffOptions.CallbackPath, StringComparison.OrdinalIgnoreCase)
    //     || path.StartsWithSegments(adminBffOptions.SignedOutCallbackPath, StringComparison.OrdinalIgnoreCase);
}