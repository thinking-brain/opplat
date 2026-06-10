namespace Opplat.Api.Admin.Extensions;

public static class AdminCorsExtensions
{
    public static IServiceCollection AddAdminCors(
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
