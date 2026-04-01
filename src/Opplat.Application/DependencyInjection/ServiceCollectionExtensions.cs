using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Features.Account.Commands;

namespace Opplat.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpplatApplication(
        this IServiceCollection services)
    {
        var appAssembly = typeof(ChangePasswordCommand).Assembly;

        // Scan the full application assembly to register MediatR infrastructure and all handlers.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(appAssembly));

        services.AddSalesApplication();
        services.AddInventoryApplication();

        // Features.Admin.* handlers are exclusively for AdminApi and require admin-only services
        // (AdminTenantCatalogDbContext, ITenantProvisioningCoordinator, IKeycloakUserService)
        // that are not available in the per-tenant MainApp. Remove them post-scan.
        var adminHandlerDescriptors = services
            .Where(sd => sd.ImplementationType?.Namespace?.StartsWith("Opplat.Application.Features.Admin") == true)
            .ToList();
        foreach (var descriptor in adminHandlerDescriptors)
            services.Remove(descriptor);

        return services;
    }
}
