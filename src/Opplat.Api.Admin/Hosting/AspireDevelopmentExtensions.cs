using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Opplat.Api.Admin.Hosting;

internal static class AspireDevelopmentExtensions
{
    public static IServiceCollection AddOpplatAspireDevelopmentSupport(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        if (environment.IsDevelopment())
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor
                    | ForwardedHeaders.XForwardedHost
                    | ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        return services;
    }

    public static WebApplication UseOpplatAspireDevelopmentSupport(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseForwardedHeaders();

        return app;
    }

    public static WebApplication UseHttpsRedirectionIfConfigured(this WebApplication app)
    {
        if (HasHttpsBinding(app.Configuration))
            app.UseHttpsRedirection();

        return app;
    }

    public static IApplicationBuilder UseHttpsRedirectionIfConfigured(
        this IApplicationBuilder app,
        IConfiguration configuration)
    {
        if (HasHttpsBinding(configuration))
            app.UseHttpsRedirection();

        return app;
    }

    public static IEndpointRouteBuilder MapOpplatHealthEndpoints(
        this IEndpointRouteBuilder endpoints,
        string serviceName,
        params string[] additionalReadyPaths)
    {
        var readyOptions = CreateHealthCheckOptions(serviceName);
        var aliveOptions = CreateHealthCheckOptions(serviceName, liveOnly: true);

        endpoints.MapHealthChecks("/health", readyOptions);
        endpoints.MapHealthChecks("/alive", aliveOptions);

        foreach (var path in additionalReadyPaths
                     .Where(path => !string.IsNullOrWhiteSpace(path))
                     .Select(NormalizePath)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            endpoints.MapHealthChecks(path, readyOptions);
        }

        return endpoints;
    }

    private static HealthCheckOptions CreateHealthCheckOptions(string serviceName, bool liveOnly = false) =>
        new()
        {
            Predicate = liveOnly
                ? registration => registration.Tags.Contains("live")
                : _ => true,
            ResponseWriter = (context, report) => context.Response.WriteAsJsonAsync(new
            {
                status = report.Status.ToString(),
                service = serviceName
            })
        };

    private static string NormalizePath(string path) =>
        path.StartsWith('/') ? path : $"/{path}";

    private static bool HasHttpsBinding(IConfiguration configuration)
    {
        if (!string.IsNullOrWhiteSpace(configuration["ASPNETCORE_HTTPS_PORT"])
            || !string.IsNullOrWhiteSpace(configuration["ASPNETCORE_HTTPS_PORTS"]))
        {
            return true;
        }

        var urls = configuration["ASPNETCORE_URLS"];
        if (string.IsNullOrWhiteSpace(urls))
            return false;

        return urls
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
    }
}
