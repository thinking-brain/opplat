using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Opplat.Application.Abstractions.Services;
using Opplat.Infrastructure.Services;
using Opplat.Infrastructure.Services.Billing;

namespace Opplat.Infrastructure.DependencyInjection;

public static class AdminInfrastructureExtensions
{
    public static IServiceCollection AddAdminInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGraphUserService(configuration);
        services.AddKeycloakUserService(configuration);
        services.AddPaymentGatewayService(configuration);
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IUserTenantResolver, UserTenantResolver>();

        services.Configure<SubscriptionRenewalWorkerOptions>(
            configuration.GetSection(SubscriptionRenewalWorkerOptions.SectionName));
        services.AddHostedService<SubscriptionRenewalWorker>();

        return services;
    }
}
