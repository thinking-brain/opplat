using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Opplat.Application.Abstractions.Services;
using Opplat.Infrastructure.Services;

namespace Opplat.Infrastructure.DependencyInjection;

public static class AdminInfrastructureExtensions
{
    public static IServiceCollection AddAdminInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGraphUserService(configuration);
        services.AddKeycloakUserService(configuration);
        services.AddScoped<IAuditLogService, AuditLogService>();

        return services;
    }
}
