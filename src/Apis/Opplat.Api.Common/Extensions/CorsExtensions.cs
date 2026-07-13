using Microsoft.Extensions.DependencyInjection;

namespace Opplat.Api.Common.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfig(
        this IServiceCollection services,
        string[] allowedCorsOrigins)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                if (allowedCorsOrigins.Length == 0)
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    return;
                }

                policy.WithOrigins(allowedCorsOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
